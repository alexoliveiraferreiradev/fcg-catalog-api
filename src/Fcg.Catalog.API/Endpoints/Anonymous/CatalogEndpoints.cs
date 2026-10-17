using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameCovers;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetPagedCatalog;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetPagedPromotedCatalogGames;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using FluentValidation;
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

            group.MapGet("/covers", GetGameCovers)
                .Produces<IEnumerable<GameCoverResponse>>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .WithSummary("Lista as capas de vários games.")
                .WithDescription("Retorna id, nome e link da capa dos games pedidos (ids=...&ids=..., até 50), inclusive desativados. Ids inexistentes são ignorados.")
                .WithName("GetCatalogGameCovers");

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

        private static async Task<IResult> GetGameCovers(
            [FromServices] ISender sender,
            [FromServices] IValidator<GetGameCoversQuery> validator,
            [FromQuery] Guid[]? ids,
            CancellationToken cancellationToken)
        {
            // Num GET o ValidationFilter não acha o command entre os argumentos; valida aqui com a mesma resposta.
            var query = new GetGameCoversQuery(ids ?? []);
            var validation = await validator.ValidateAsync(query, cancellationToken);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ids"] = validation.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var response = await sender.Send(query, cancellationToken);
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
