using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Configurations;
using RealTaskManager.GraphQL.Extensions;

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

app.UseRouting();
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
    var access_token = generator.GenerateToken(request.Email, userRoles);
    return Results.Ok(access_token);
});

app.MapPost("/custom-register", async (
    UserManager<TaskManagerUser> userManager,
    RegisterRequest request, string username) =>
{
    var user = new TaskManagerUser { Email = request.Email,  UserName = username };
    var result = await userManager.CreateAsync(user, request.Password);

    return result.Succeeded ? Results.Created() : Results.BadRequest("Registration failed");
});

app.MapPost("/add-role", async (
    string role,
    RoleManager<IdentityRole> roleManager) =>
{
    if (await roleManager.RoleExistsAsync(role)) return Results.Conflict("Role already exists");
    var result = await roleManager.CreateAsync(new IdentityRole(role));
    return result.Succeeded ? Results.Created() : Results.BadRequest("Role not created");
});

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
});

app.MapGet("/weather", () => "The weather is nice").RequireAuthorization("AdminPolicy");

app.RunWithGraphQLCommands(args);