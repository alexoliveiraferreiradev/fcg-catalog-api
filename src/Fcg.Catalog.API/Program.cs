using Fcg.Catalog.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenApiExtension()
    .AddServicesExtensions();

var app = builder.Build();
await app.SeedData();

app.AddAppConfiguration()
    .UseSwaggerDocumentation()
    .MapEndpoints();
app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/readiness", new HealthCheckOptions { Predicate = _ => true });

app.Run();
