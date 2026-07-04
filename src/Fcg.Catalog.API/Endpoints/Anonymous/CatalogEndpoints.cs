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
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Lista os games ativos do catálogo.")
                .WithDescription("Retorna uma lista paginada de todos os games ativos no catálogo, contendo título, gênero e preço original ou promocional. Permite filtro por gênero.")
                .WithName("GetCatalogGames");         

            group.MapGet("/promoted", GetGamesByPromotion)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Lista os games em promoção ativa.")
                .WithDescription("Retorna uma lista paginada de todos os games ativos que possuem uma promoção em vigência no momento, permitindo filtro opcional por gênero.")
                .WithName("GetPromotedCatalogGames");

        }


        private static async Task<IResult> GetAllGames(
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] GameGenre? genre,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellationToken);

            var query = new GetPagedCatalogQuery(page, pageSize,genre);
            var response = await sender.Send(query,cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }      

        private static async Task<IResult> GetGamesByPromotion(
            [FromServices] ISender sender,
            [FromQuery] GameGenre? genre,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellationToken);

            var query = new GetPagedPromotedCatalogGamesQuery() with
            {
                Page = page,
                PageSize = pageSize,
                Genre = genre,
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
