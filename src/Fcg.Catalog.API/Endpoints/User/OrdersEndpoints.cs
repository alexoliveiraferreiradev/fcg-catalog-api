using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Catalog.API.Endpoints.User
{
    public static class OrdersEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuario/catalogo").RequireAuthorization("AcessoGeral").WithTags("Compra");

            group.MapPost("/carrinho", PlaceOrder)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound); 
        }

        private static async Task<IResult> PlaceOrder(
            [FromServices] ISender sender,
            [FromBody] RealizarPedidoCommand realizarPedidoCommand,
            CancellationToken cancellationToken,
            ClaimsPrincipal user)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }

            var UserId = Guid.Parse(currentUserIdClaim);

            var pedidoJogoCommand = realizarPedidoCommand with
            {
                UserId = UserId,
                NomeUsuario = user.FindFirstValue(ClaimTypes.Name),
                EmailUsuario = user.FindFirstValue(ClaimTypes.Email),
            };

            var response = await sender.Send(pedidoJogoCommand);

            if(!response)
                return Results.BadRequest();

            return Results.Ok();
        }

    }
}
