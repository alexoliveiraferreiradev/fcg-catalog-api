using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarPromocao;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarPromocaoInvalida;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemCatalogoJogosPromovidosPaginado;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterPromocaoPorJogoId;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Fcg.Catalogo.API.Endpoints.Admin
{
    public static class GerenciaPromocaoEndpoint
    {
        public static void MapGerenciaPromocaoEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/promocao").RequireAuthorization().WithTags("Gerenciamento de Promoções");

            group.MapGet("/obtem-promovidos", ObtemJogosPromovidosPaginados)
                .Produces<JogoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/obtem-por-id/{promocaoId:guid}", ObtemPromocaoPorId)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/cria-promocao", CriaPromocao)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/desativar/{promocaoId:guid}", DesativarPromocao)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

        }


        private static async Task<IResult> ObtemJogosPromovidosPaginados(
            [FromServices] ISender sender,
            CancellationToken cancellation,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10
            )
        {
            var query = new ObtemCatalogoJogosPromovidosQuery(pagina, tamanho);
            var response = await sender.Send(query, cancellation);

            if (response == null || !response.Items.Any())
            {
                return Results.NotFound(new { Message = "Nenhum jogo promovido encontrado." });
            }

            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemPromocaoPorId(
            [FromRoute] Guid promocaoId,
            [FromServices] ISender sender,
            [FromServices] ObterPromocaoPorJogoIdQuery obterPromocaoPorJogoIdQuery,
            [FromServices] DesativarPromocaoInvalidaCommand desativarPromocaoInvalidaCommand,
            CancellationToken cancellation
            )
        {
            await sender.Send(desativarPromocaoInvalidaCommand, cancellation);

            var query = new ObterPromocaoPorJogoIdQuery(promocaoId);

            var response = await sender.Send(query, cancellation);

            if (response == null)
            {
                return Results.NotFound(new { Message = "Promoção não encontrada." });
            }

            return Results.Ok(response);
        }


        private static async Task<IResult> CriaPromocao(
            [FromBody] AdicionarPromocaoJogoCommand adicionarPromocaoJogoCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var response = await sender.Send(adicionarPromocaoJogoCommand, cancellationToken);
            return Results.Created($"/api/admin/promocao/{response.PromocaoId}", response);
        }


        private static async Task<IResult> DesativarPromocao(
            [FromRoute] Guid promocaoId,
            [FromServices] DesativarPromocaoCommand desativarPromocaoCommand, 
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = desativarPromocaoCommand with { PromocaoId = promocaoId };
            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }
    }
}

