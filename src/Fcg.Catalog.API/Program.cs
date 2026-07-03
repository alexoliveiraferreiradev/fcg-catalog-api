using Fcg.Catalog.API.Extensions;
using Fcg.Core.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenApiExtension()
    .AddServicesExtensions();

var app = builder.Build();
await app.SeedData();

app.AddAppConfiguration()
    .UseSwaggerDocumentation()
    .MapEndpoints();

app.Run();
