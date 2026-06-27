using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoJogosPromovidosPaginado;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoPaginados;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoPorGeneroPaginado;
using Fcg.Catalogo.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalogo.API.Endpoints.Anonymous
{
    public static class CatalogoJogosEndpoint
    {
        public static void MapCatalogoJogosEndpoint(this WebApplication app)
        {

            var group = app.MapGroup("/api/catalogo-jogos").WithTags("Catálogo de Jogos").AllowAnonymous();

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
            [FromServices] ISender mediator,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            var query = new ObtemCatalogoPaginadosQuery(pagina, tamanho);
            var response = await mediator.Send(query);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemJogosPorGenero(
            [FromServices] ISender mediator,
            [FromRoute] GeneroJogo genero,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            var query = new ObtemCatalogoPorGeneroQuery() with
            {
                Pagina = pagina,
                TamanhoPagina = tamanho,
                Genero = genero
            };
            var response = await mediator.Send(query);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemJogosPromovidos(
            [FromServices] ISender mediator,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            var query = new ObtemCatalogoJogosPromovidosQuery() with
            {
                Pagina = pagina,
                TamanhoPagina = tamanho,
                ApenasPromovidos = true
            };
            var response = await mediator.Send(query);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }
    }
}
