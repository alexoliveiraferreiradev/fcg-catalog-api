using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedPromotedCatalogGames;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetPromotionByGameId;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Fcg.Catalog.API.Filters;


namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class PromotionsEndpoints
    {
        public static void MapPromotionsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/promotions").RequireAuthorization().WithTags("Admin - Gerenciamento de Promoções");

            group.MapGet("", GetPagedCatalogGamePromotion)
                .Produces<GameResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Lista promoções de games de forma paginada.")
                .WithDescription("Retorna a lista de promoções de games de forma paginada contendo informações sobre percentual de desconto e validade.")
                .WithName("AdminGetPagedCatalogGamePromotion");

            group.MapGet("/{PromotionId:guid}", GetPromotionById)
                .Produces<PromotionResponse>()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Obtém uma promoção por ID.")
                .WithDescription("Busca os detalhes de uma promoção específica a partir do seu identificador único (GUID).")
                .WithName("AdminGetPromotionById");

            group.MapPost("", CreatePromotion)
                .AddEndpointFilter<ValidationFilter<AddPromotionGameCommand>>()
                .Produces<PromotionResponse>()
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem()
                .WithSummary("Cria uma nova promoção para um game.")
                .WithDescription("Cadastra uma nova promoção associada a um game, aplicando um percentual de desconto com datas de início e término.")
                .WithName("AdminCreatePromotion");

            group.MapPut("/{PromotionId:guid}/deactivate", DeactivatePromotion)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Desativa uma promoção existente.")
                .WithDescription("Desativa uma promoção de forma lógica antes do seu prazo de expiração.")
                .WithName("AdminDeactivatePromotion");

        }


        private static async Task<IResult> GetPagedCatalogGamePromotion(
            [FromServices] ISender sender,
            CancellationToken cancellation,
            [FromQuery] int Page = 1,
            [FromQuery] int PageSize = 10
            )
        {
            var query = new GetPagedPromotedCatalogGamesQuery(Page, PageSize);
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

