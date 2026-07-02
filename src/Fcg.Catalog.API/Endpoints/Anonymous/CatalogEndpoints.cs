using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalog;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalog.API.Endpoints.Anonymous
{
    public static class CatalogEndpoints
    {
        public static void MapCatalogEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/catalog/games").WithTags("Catálogo de Games").AllowAnonymous();

            group.MapGet("", GetAllGames)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);         

            group.MapGet("/promoted", GetGamesByPromotion)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

        }


        private static async Task<IResult> GetAllGames(
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] GameGenre? genre,
            [FromQuery] int Page = 1,
            [FromQuery] int PageSize = 10)
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellationToken);

            var query = new GetPagedCatalogQuery(Page, PageSize,genre);
            var response = await sender.Send(query,cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }      

        private static async Task<IResult> GetGamesByPromotion(
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] int Page = 1,
            [FromQuery] int PageSize = 10)
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellationToken);

            var query = new GetPagedPromotedCatalogGamesQuery() with
            {
                Page = Page,
                PageSize = PageSize,
                OnlyPromoted = true
            };
            var response = await sender.Send(query, cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }
    }
}
