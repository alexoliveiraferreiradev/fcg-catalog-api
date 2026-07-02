using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalog;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalogByGenre;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalog.API.Endpoints.Anonymous
{
    public static class CatalogEndpoints
    {
        public static void MapCatalogEndpoints(this WebApplication app)
        {

            var group = app.MapGroup("/api/catalogo-jogos").WithTags("Catálogo de Games").AllowAnonymous();

            group.MapGet("/obtem-todos", GetAllGames)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);


            group.MapGet("obtem-por-genero/{Genre}", GetGamesByGenre)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);


            group.MapGet("obtem-promovidos", GetGamesByPromotion)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

        }


        private static async Task<IResult> GetAllGames(
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellationToken);

            var query = new ObtemCatalogPaginadosQuery(pagina, tamanho);
            var response = await sender.Send(query,cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> GetGamesByGenre(
            [FromServices] ISender sender,
            [FromRoute] GameGenre Genre,
            CancellationToken cancellationToken,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellationToken);

            var query = new ObtemCatalogPorGeneroQuery() with
            {
                Pagina = pagina,
                TamanhoPagina = tamanho,
                Genre = Genre
            };
            var response = await sender.Send(query, cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> GetGamesByPromotion(
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellationToken);

            var query = new ObtemCatalogJogosPromovidosQuery() with
            {
                Pagina = pagina,
                TamanhoPagina = tamanho,
                ApenasPromovidos = true
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
