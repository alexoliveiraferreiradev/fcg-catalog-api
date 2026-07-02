using Fcg.Catalog.API.Endpoints.Admin;
using Fcg.Catalog.API.Endpoints.Anonymous;
using Fcg.Catalog.API.Endpoints.Usuario;

namespace Fcg.Catalog.API.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapGerenciaJogoEndpoint();
            app.MapGerenciaPromocaoEndpoint();
            app.MapCatalogJogosEndpoint();
            app.MapBibliotecaUsuarioPaginadaEndpoint();
            app.MapAdquirirJogo();
            return app;
        }
}
}
