using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fcg.Catalog.API.Filters
{
    public class OrderTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var orderedTags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Catálogo de Games", Description = "Endpoints públicos para consulta de catálogo e promoções de jogos." },
                new OpenApiTag { Name = "Biblioteca do Usuário", Description = "Endpoints para consulta de jogos adquiridos pelo próprio usuário." },
                new OpenApiTag { Name = "Compras e Pedidos", Description = "Endpoints para realização,processamento de compras de jogos e histórico de pedidos." },
                new OpenApiTag { Name = "Admin - Gerenciamento de Games", Description = "Endpoints administrativos para cadastro, edição e ativação/desativação de jogos." },
                new OpenApiTag { Name = "Admin - Gerenciamento de Promoções", Description = "Endpoints administrativos para criação, controle e desativação de promoções." }
            };

            var existingTags = swaggerDoc.Tags != null ? swaggerDoc.Tags.ToList() : new List<OpenApiTag>();
            var finalTags = new List<OpenApiTag>(orderedTags);

            foreach (var tag in existingTags)
            {
                if (!finalTags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    finalTags.Add(tag);
                }
            }

            swaggerDoc.Tags = finalTags;
        }
    }
}
