using RealTaskManager.GraphQL.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceConfigs(builder);

builder.AddGraphQL().AddTypes()
    .AddGlobalObjectIdentification()
    .AddDbContextCursorPagingProvider()
    .AddPagingArguments()
    .AddMutationConventions();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);