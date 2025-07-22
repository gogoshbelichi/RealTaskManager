using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AuthEndpoints;
using RealTaskManager.GraphQL.Configurations;
using RealTaskManager.GraphQL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<MagicOptions>(
    builder.Configuration.GetSection("MagicOptions"));

builder.Services.AddScoped<CustomUserService>();
builder.Services.AddScoped<TokenService>();
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

builder.AddGraphQL()
    .AddMaxExecutionDepthRule(10).AddTypes()
    .AddGlobalObjectIdentification()
    .AddQueryContext()
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
app.MapAuthEndpoints();

app.MapGet("/weather", () => "The weather is nice").RequireAuthorization("UserPolicy");

if (app.Environment.IsDevelopment())
{
    await FakeDataSeeder.SeedAsync(app.Services);
}

app.RunWithGraphQLCommands(args);