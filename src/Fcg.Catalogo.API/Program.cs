using Fcg.Catalogo.API.Endpoints.Admin;
using Fcg.Catalogo.API.Endpoints.Anonymous;
using Fcg.Catalogo.API.Endpoints.Usuario;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.AdicionarJogo;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Infrastructure.Persistence;
using Fcg.Catalogo.Infrastructure.Repository;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.WebApi.Security;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP Cloud Games API",
        Version = "v1",
        Description = "API para gestão de catálogo de jogos e processamento de pedidos.",
        Contact = new OpenApiContact
        {
            Name = "Alex Oliveira Ferreira"
        }
    });
});
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("CatalogoConnection");
builder.Services.AddDbContext<CatalogoDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.AddEntityFrameworkOutbox<CatalogoDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AdicionarJogoCommand).Assembly);
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssembly(typeof(AdicionarJogoCommand).Assembly);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<CatalogoDbContext>().Database.GetDbConnection());
builder.Services.AddScoped<CatalogoDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IJogoRepository, JogoRepository>();  
builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CatalogoDbContext>();
        await CatalogoDbContextSeed.SeedDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao alimentar o banco de dados inicial.");
    }
}


app.MapGerenciaJogoEndpoint();
app.MapGerenciaPromocaoEndpoint();
app.MapCatalogoJogosEndpoint();
app.MapBibliotecaUsuarioPaginadaEndpoint();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiap Cloud Games");
});

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.Run();
