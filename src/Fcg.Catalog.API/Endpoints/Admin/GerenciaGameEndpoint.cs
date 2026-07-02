using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarJogo;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarJogo;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocaoInvalida;
using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReativarJogo;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObtemTodosJogos;
using Fcg.Catalog.Application.Features.Catalog.Queries.ObterJogoPorId;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalog.API.Endpoints.Admin
{
    public static class GerenciaJogoEndpoint
    {
        public static void MapGerenciaJogoEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/Game").RequireAuthorization().WithTags("Gerenciamento de Games");

            group.MapGet("/obtem-por-id/{GameId:guid}", ObtemJogoPorId)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/obtem-todos/", ObtemTodosJogos)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/Add", AdicionarJogo)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/Deactivate/{GameId:guid}", DesativarJogo)
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound); 

            group.MapPut("/Update/{GameId:guid}", AtualizarJogo)
             .Produces<JogoResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/Reactivate/{GameId:guid}", ReativarJogo)
            .Produces<JogoResponse>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }


        private static async Task<IResult> ObtemJogoPorId(
            [FromRoute] Guid GameId, [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(new DesativarPromocaoInvalidaCommand(), cancellationToken);

            var query = new GetGameByIdQuery(GameId);
            var response = await sender.Send(query, cancellationToken);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> ObtemTodosJogos(
            [FromServices] ISender mediator)
        {
            var query = new ObtemTodosJogosQuery();
            var response = await mediator.Send(query);
            if (response == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(response);
        }


        private static async Task<IResult> AdicionarJogo(
            [FromServices] ISender sender,
            [FromBody] AddGameCommand AddGameCommand,
            CancellationToken cancellationToken)
        {
            var response = await sender.Send(AddGameCommand, cancellationToken);
            return Results.Created($"/api/admin/Game/obtem-por-id/{response.Id}", response);
        }

        private static async Task<IResult> DesativarJogo(
            [FromBody] DesativarJogoCommand desativarJogo,
            [FromRoute] Guid GameId,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = new DesativarJogoCommand(GameId);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> AtualizarJogo(
            [FromRoute] Guid GameId,
            [FromBody] UpdateGameCommand UpdateGameCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command = UpdateGameCommand with { GameId = GameId };
            var response = await sender.Send(command, cancellationToken);
            return Results.Ok(response);
        }

        private static async Task<IResult> ReativarJogo(
            [FromRoute] Guid GameId,
            [FromServices] ReativarJogoCommand reativarJogoCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command =  reativarJogoCommand with
            {
                GameId = GameId,    
            };

            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }

    }
}
