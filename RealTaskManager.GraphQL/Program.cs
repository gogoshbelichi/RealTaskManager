using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Configurations;
using RealTaskManager.GraphQL.Extensions;
using RealTaskManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<MagicOptions>(
    builder.Configuration.GetSection("MagicOptions"));

builder.Services.AddSingleton<TokenGenerator>();
var magicOptions = builder.Configuration.GetSection("MagicOptions").Get<MagicOptions>() ?? 
                   throw new ArgumentNullException("Something went wrong");
builder.Services.AddAuthentication(authenticationOptions =>
{
    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    
}).AddJwtBearer(j =>
{
    j.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(magicOptions.MagicString)
        ),
        ValidAudience = magicOptions.MagicAudience,
        ValidIssuer = magicOptions.MagicIssuer,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
    };
});

builder.Services.AddAuthorizationBuilder();

builder.AddCustomIdentity();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.Last());
});

builder.Services.AddServiceConfigs(builder);

builder.AddGraphQL().AddTypes()
    .AddGlobalObjectIdentification()
    .AddDbContextCursorPagingProvider()
    .AddPagingArguments()
    .AddFiltering()
    .AddSorting()
    .AddMutationConventions()
    .AddAuthorization(policy =>
    {
        policy.AddPolicy("AdminPolicy", policyBuilder =>
        {
            policyBuilder.RequireRole("Administrator");
        });
        policy.AddPolicy("UserPolicy", policyBuilder =>
        {
            policyBuilder.RequireRole("User");
        });
    });

var app = builder.Build();

await app.UseCustomRoles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.MapIdentityApi<TaskManagerUser>();

app.MapPost("/custom-login", async (
    LoginRequest request,
    TokenGenerator generator,
    UserManager<TaskManagerUser> userManager) =>
{
    var user = await userManager.FindByEmailAsync(request.Email);
    if (user is null) return Results.BadRequest("User not found");
    if (!await userManager.CheckPasswordAsync(user, request.Password))
        return Results.Unauthorized();
    var userRoles = await userManager.GetRolesAsync(user);
    var userId = user.Id;
    var access_token = generator.GenerateToken(request.Email, userId, userRoles);
    return Results.Ok(access_token);
});

app.MapPost("/custom-register", async (
    UserManager<TaskManagerUser> userManager,
    RealTaskManagerDbContext dbContext,
    RegisterRequest request, string username) =>
{
    var user = new TaskManagerUser { Email = request.Email,  UserName = username };
    var result = await userManager.CreateAsync(user, request.Password);
    if (!result.Succeeded)
    {
        return Results.BadRequest($"Registration failed {result.Errors}");
    }

    var userEntity = new UserEntity
    {
        IdentityId = user.Id
    };
    
    await dbContext.Users.AddAsync(userEntity);
    await dbContext.SaveChangesAsync();
    
    return Results.Created();
});

app.MapPost("/add-role", async (
    string role,
    RoleManager<IdentityRole> roleManager) =>
{
    if (await roleManager.RoleExistsAsync(role)) return Results.Conflict("Role already exists");
    var result = await roleManager.CreateAsync(new IdentityRole(role));
    return result.Succeeded ? Results.Created() : Results.BadRequest("Role not created");
}).RequireAuthorization("AdminPolicy");

app.MapPatch("/assign-role", async (
    string role,
    string email,
    UserManager<TaskManagerUser> userManager
    ) =>
{
    var user = await userManager.FindByEmailAsync(email);
    
    if (user is null) return Results.BadRequest("User not found");
    var result = await userManager.AddToRoleAsync(user, role);
    return result.Succeeded ? Results.Accepted() : Results.BadRequest("Role not added");
}).RequireAuthorization();

app.MapGet("/weather", () => "The weather is nice").RequireAuthorization("UserPolicy");

app.RunWithGraphQLCommands(args);