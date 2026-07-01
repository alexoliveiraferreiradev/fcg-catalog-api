using Fcg.Catalogo.API.Endpoints.Admin;
using Fcg.Catalogo.API.Endpoints.Anonymous;
using Fcg.Catalogo.API.Endpoints.Usuario;

namespace Fcg.Catalogo.API.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapGerenciaJogoEndpoint();
            app.MapGerenciaPromocaoEndpoint();
            app.MapCatalogoJogosEndpoint();
            app.MapBibliotecaUsuarioPaginadaEndpoint();
            app.MapAdquirirJogo();
            return app;
        }
}
}
