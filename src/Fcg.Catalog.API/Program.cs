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
app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/readiness", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
