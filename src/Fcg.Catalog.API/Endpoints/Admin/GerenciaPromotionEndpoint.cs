using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocaoInvalida;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObtemCatalogJogosPromovidosPaginado;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObterPromocaoPorJogoId;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class GerenciaPromocaoEndpoint
    {
        public static void MapGerenciaPromocaoEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/Promotion").RequireAuthorization().WithTags("Gerenciamento de Promoções");

            group.MapGet("/obtem-promovidos", ObtemJogosPromovidosPaginados)
                .Produces<JogoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/obtem-por-id/{PromotionId:guid}", ObtemPromocaoPorId)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/cria-Promotion", CriaPromocao)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/Deactivate/{PromotionId:guid}", DeactivatePromotion)
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
            var query = new ObtemCatalogJogosPromovidosQuery(pagina, tamanho);
            var response = await sender.Send(query, cancellation);

            if (response == null || !response.Items.Any())
            {
                return Results.NotFound(new { Message = "Nenhum Game promovido encontrado." });
            }

            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemPromocaoPorId(
            [FromRoute] Guid PromotionId,
            [FromServices] ISender sender,
            CancellationToken cancellation
            )
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellation);

            var query = new ObterPromocaoPorJogoIdQuery(PromotionId);

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
            return Results.Created($"/api/admin/Promotion/{response.PromotionId}", response);
        }


        private static async Task<IResult> DeactivatePromotion(
            [FromRoute] Guid PromotionId,
            [FromServices] DesativarPromocaoCommand desativarPromocaoCommand, 
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = desativarPromocaoCommand with { PromotionId = PromotionId };
            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }
    }
}

