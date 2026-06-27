using Fcg.Catalogo.Application.Features.Catalogo.Commands.AdicionarJogo;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.AtualizarJogo;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarJogo;
using Fcg.Catalogo.Application.Features.Catalogo.Commands.ReativarJogo;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObtemTodosJogos;
using Fcg.Catalogo.Application.Features.Catalogo.Queries.ObterJogoPorId;
using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Catalogo.API.Endpoints.Admin
{
    public static class GerenciaJogoEndpoint
    {
        public static void MapGerenciaJogoEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/admin/jogo").RequireAuthorization().WithTags("Gerenciamento de Jogos");

            group.MapGet("/obtem-por-id/{jogoId:guid}", ObtemJogoPorId)
             .Produces<JogosResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/obtem-todos/", ObtemTodosJogos)
             .Produces<JogosResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/adicionar", AdicionarJogo)
             .Produces<JogosResponse>()
             .Produces(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/desativar/{jogoId:guid}", DesativarJogo)
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound); 

            group.MapPut("/atualizar/{jogoId:guid}", AtualizarJogo)
             .Produces<JogosResponse>()
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/reativar/{jogoId:guid}", ReativarJogo)
            .Produces<JogosResponse>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }


        private static async Task<IResult> ObtemJogoPorId(
            [FromRoute] Guid jogoId, [FromServices] ISender mediator)
        {
            var query = new ObterJogoPorIdQuery(jogoId);
            var response = await mediator.Send(query);
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
            [FromBody] AdicionarJogoCommand adicionarJogoCommand,
            CancellationToken cancellationToken)
        {
            var response = await sender.Send(adicionarJogoCommand, cancellationToken);
            return Results.Created($"/api/admin/jogo/obtem-por-id/{response.Id}", response);
        }

        private static async Task<IResult> DesativarJogo(
            [FromBody] DesativarJogoCommand desativarJogo,
            [FromRoute] Guid jogoId,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
            )
        {
            var command = new DesativarJogoCommand(jogoId);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> AtualizarJogo(
            [FromRoute] Guid jogoId,
            [FromBody] AtualizarJogoCommand atualizarJogoCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command = atualizarJogoCommand with { JogoId = jogoId };
            var response = await sender.Send(command, cancellationToken);
            return Results.Ok(response);
        }

        private static async Task<IResult> ReativarJogo(
            [FromRoute] Guid jogoId,
            [FromServices] ReativarJogoCommand reativarJogoCommand,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var command =  reativarJogoCommand with
            {
                JogoId = jogoId,    
            };

            await sender.Send(command, cancellationToken);
            return Results.NoContent();
        }

    }
}
