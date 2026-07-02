using Fcg.Catalog.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenApiExtension()
    .AddServicesExtensions()
    .AddDependencyInjection();

var app = builder.Build();
await app.SeedData();

app.UseSecurityMiddlewares()
    .UseSwaggerDocumentation()
    .MapEndpoints();

app.Run();
