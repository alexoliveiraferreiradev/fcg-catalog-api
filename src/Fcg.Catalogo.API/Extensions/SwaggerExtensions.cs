using Microsoft.OpenApi.Models;

namespace Fcg.Catalogo.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static WebApplicationBuilder AddOpenApiExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
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
            return builder;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fiap Cloud Games");
            });
            return app;
        }
    }
}
