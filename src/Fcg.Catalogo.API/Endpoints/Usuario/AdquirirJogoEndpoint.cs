using Fcg.Catalogo.Application.Features.Catalogo.Commands.AcessoGeral.AdquirirJogo;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Catalogo.API.Endpoints.Usuario
{
    public static class AdquirirJogoEndpoint
    {
        public static void MapAdquirirJogo(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuario/catalogo-jogos").RequireAuthorization("AcessoGeral").WithTags("Compra");

            group.MapPost("/carrinho", AdquirirJogo)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound); 
        }

        private static async Task<IResult> AdquirirJogo(
            [FromServices] ISender sender,
            [FromBody] AdquirirJogoCommand adquirirJogoCommand,
            CancellationToken cancellationToken,
            ClaimsPrincipal user)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }

            var usuarioId = Guid.Parse(currentUserIdClaim);

            var pedidoJogoCommand = adquirirJogoCommand with
            {
                UsuarioId = usuarioId
            };

            var response = await sender.Send(pedidoJogoCommand);

            if(!response)
                return Results.BadRequest();

            return Results.Ok();
        }

    }
}
