using Fcg.Catalog.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServicesExtensions();

var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    await app.SeedData();   
}
app.AddAppConfiguration();
app.Run();
