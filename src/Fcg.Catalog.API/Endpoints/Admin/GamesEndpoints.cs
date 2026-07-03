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
using Fcg.Catalog.API.Filters;

namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class GamesEndpoints
    {
        public static void MapGamesEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/games").RequireAuthorization().WithTags("Admin - Gerenciamento de Games");

            group.MapGet("/{GameId:guid}", GetGameById)
             .Produces<GameResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Obtém um game por ID para o administrador.")
             .WithDescription("Busca um game específico pelo seu identificador único (GUID) e retorna seus detalhes cadastrais completos.")
             .WithName("AdminGetGameById");

            group.MapGet("", GetAllGames)
             .Produces<GameResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Lista todos os games cadastrados.")
             .WithDescription("Retorna a lista completa de todos os games cadastrados no catálogo, incluindo os ativos e inativos.")
             .WithName("AdminGetAllGames");

            group.MapPost("", AddGame)
             .AddEndpointFilter<ValidationFilter<AddGameCommand>>()
             .Produces<GameResponse>()
             .Produces(StatusCodes.Status201Created)
             .ProducesValidationProblem()
             .WithSummary("Cadastra um novo game.")
             .WithDescription("Realiza a inserção de um novo game no catálogo informando título, gênero, preço e status.")
             .WithName("AdminAddGame");

            group.MapPut("/{GameId:guid}/deactivate", DeactiveGame)
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Desativa um game no catálogo.")
             .WithDescription("Realiza a desativação lógica de um game pelo seu ID, impedindo novas compras e sua exibição no catálogo público.")
             .WithName("AdminDeactivateGame"); 

            group.MapPut("/{GameId:guid}", UpdateGame)
             .AddEndpointFilter<ValidationFilter<UpdateGameCommand>>()
             .Produces<GameResponse>()
             .Produces(StatusCodes.Status200OK)
             .ProducesValidationProblem()
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Atualiza os dados de um game.")
             .WithDescription("Permite atualizar as informações cadastrais de um game existente (título, preço, gênero, descrição).")
             .WithName("AdminUpdateGame");

            group.MapPut("/{GameId:guid}/activate", ReactiveGame)
             .Produces<GameResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Reativa um game desativado.")
             .WithDescription("Restaura o status ativo de um game desativado anteriormente, tornando-o novamente disponível no catálogo público.")
             .WithName("AdminReactivateGame");
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
