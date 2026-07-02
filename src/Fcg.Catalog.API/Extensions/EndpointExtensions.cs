using Fcg.Catalog.API.Endpoints.Admin;
using Fcg.Catalog.API.Endpoints.Anonymous;
using Fcg.Catalog.API.Endpoints.User;

namespace Fcg.Catalog.API.Extensions
{
    public static class EndpointExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapGamesEndpoints();
            app.MapPromotionsEndpoints();
            app.MapCatalogEndpoints();
            app.MapLibraryUserEndpoints();
            app.MapOrderEndpoints();
            return app;
        }
}
}
