using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReactivateGame;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetAllGames;
using Fcg.Catalog.Application.Features.Catalog.Queries.GetGameById;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class GamesEndpoints
    {
        public static void MapGamesEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/games").RequireAuthorization().WithTags("Gerenciamento de Games");

            group.MapGet("/games/{GameId:guid}", GetGameById)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("", GetAllGames)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("", AddGame)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{GameId:guid}/deactivate", DeactiveGame)
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound); 

            group.MapPut("/{GameId:guid}", UpdateGame)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/{GameId:guid}/activate", ReactiveGame)
            .Produces<JogoResponse>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }


        private static async Task<IResult> GetGameById(
            [FromRoute] Guid GameId, [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(new DeactivatePromotionInvalidaCommand(), cancellationToken);

            var query = new GetGameByIdQuery(GameId);
            var response = await sender.Send(query, cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> GetAllGames(
            [FromServices] ISender mediator)
        {
            var query = new GetAllGamesQuery();
            var response = await mediator.Send(query);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }


        private static async Task<IResult> AddGame(
            [FromServices] ISender sender,
            [FromBody] AddGameCommand AddGameCommand,
            CancellationToken cancellationToken)
        {
            var response = await sender.Send(AddGameCommand, cancellationToken);
            return Results.Created($"/api/admin/games/{response.Id}", response);
        }

        private static async Task<IResult> DeactiveGame(
            [FromBody] DeactivateGameCommand DeactivateGame,
            [FromRoute] Guid GameId,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = new DeactivateGameCommand(GameId);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> UpdateGame(
            [FromRoute] Guid GameId,
            [FromBody] UpdateGameCommand UpdateGameCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command = UpdateGameCommand with { GameId = GameId };
            var response = await sender.Send(command, cancellationToken);
            return Results.Ok(response);
        }

        private static async Task<IResult> ReactiveGame(
            [FromRoute] Guid GameId,
            [FromServices] ReactivateGameCommand ReactivateGameCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command =  ReactivateGameCommand with
            {
                GameId = GameId,    
            };

            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }

    }
}
