using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocaoInvalida;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogJogosPromovidosPaginado;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogPaginados;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogPorGeneroPaginado;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalog.API.Endpoints.Anonymous
{
    public static class CatalogJogosEndpoint
    {
        public static void MapCatalogJogosEndpoint(this WebApplication app)
        {

            var group = app.MapGroup("/api/Catalog-jogos").WithTags("Catálogo de Jogos").AllowAnonymous();

            group.MapGet("/obtem-todos",ObtemTodosJogos)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);


            group.MapGet("obtem-por-genero/{genero}",ObtemJogosPorGenero)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);


            group.MapGet("obtem-promovidos", ObtemJogosPromovidos)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

        }


        private static async Task<IResult> ObtemTodosJogos(
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

        private static async Task<IResult> ObtemJogosPorGenero(
            [FromServices] ISender sender,
            [FromRoute] GeneroJogo genero,
            CancellationToken cancellationToken,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellationToken);

            var query = new ObtemCatalogPorGeneroQuery() with
            {
                Pagina = pagina,
                TamanhoPagina = tamanho,
                Genero = genero
            };
            var response = await sender.Send(query, cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemJogosPromovidos(
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
