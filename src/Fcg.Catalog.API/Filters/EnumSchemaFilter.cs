using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fcg.Catalog.API.Filters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumValues = Enum.GetValues(context.Type)
                    .Cast<int>()
                    .Select(v => $"{Enum.GetName(context.Type, v)} = {v}");

                schema.Description += "<p>Valores disponíveis:</p><ul><li>" +
                                      string.Join("</li><li>", enumValues) +
                                      "</li></ul>";
            }
        }
    }
}
