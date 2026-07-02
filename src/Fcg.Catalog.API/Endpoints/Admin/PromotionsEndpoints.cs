using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPromotionByGameId;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class PromotionsEndpoints
    {
        public static void MapPromotionsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/promotions").RequireAuthorization().WithTags("Gerenciamento de Promoções");

            group.MapGet("", GetPagedCatalogGamePromotion)
                .Produces<JogoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/{PromotionId:guid}", GetPromotionById)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("", CreatePromotion)
                .Produces<PromocaoResponse>()
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{PromotionId:guid}/deactivate", DeactivatePromotion)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

        }


        private static async Task<IResult> GetPagedCatalogGamePromotion(
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

        private static async Task<IResult> GetPromotionById(
            [FromRoute] Guid PromotionId,
            [FromServices] ISender sender,
            CancellationToken cancellation
            )
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellation);

            var query = new GetPromotionByGameIdQuery(PromotionId);

            var response = await sender.Send(query, cancellation);

            if (response == null)
            {
                return Results.NotFound(new { Message = "Promoção não encontrada." });
            }

            return Results.Ok(response);
        }


        private static async Task<IResult> CreatePromotion(
            [FromBody] AddPromotionGameCommand AddPromotionGameCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var response = await sender.Send(AddPromotionGameCommand, cancellationToken);
            return Results.Created($"/api/admin/promocao/obtem-por-id/{response.PromotionId}", response);
        }


        private static async Task<IResult> DeactivatePromotion(
            [FromRoute] Guid PromotionId,
            [FromServices] DeactivatePromotionCommand DeactivatePromotionCommand, 
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = DeactivatePromotionCommand with { PromotionId = PromotionId };
            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }
    }
}

