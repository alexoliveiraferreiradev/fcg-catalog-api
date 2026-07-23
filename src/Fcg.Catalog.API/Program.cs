using Fcg.Catalog.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenApiExtension()
    .AddServicesExtensions();

var app = builder.Build();
await app.SeedData();
app.AddAppConfiguration();
app.Run();
