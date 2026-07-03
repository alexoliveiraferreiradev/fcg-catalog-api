using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Fcg.Catalog.API.Filters;

namespace Fcg.Catalog.API.Endpoints.User
{
    public static class OrdersEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/orders").RequireAuthorization("GeneralAccess").WithTags("Compras");

            group.MapPost("", PlaceOrder)
                .AddEndpointFilter<ValidationFilter<PlaceOrderCommand>>()
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Realiza a compra de um game.")
                .WithDescription("Registra uma nova intenção de compra para um game do catálogo. O ID, nome e e-mail do usuário comprador são extraídos de forma segura a partir das claims do token JWT.")
                .WithName("PlaceOrder"); 
        }

        private static async Task<IResult> PlaceOrder(
            [FromServices] ISender sender,
            [FromBody] IEnumerable<Guid> JogosIds,
            CancellationToken cancellationToken,
            ClaimsPrincipal user)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(currentUserIdClaim);

            var orderCommand = new PlaceOrderCommand(userId, user.FindFirstValue(ClaimTypes.Name), user.FindFirstValue(ClaimTypes.Email), JogosIds);

            var response = await sender.Send(orderCommand);

            if(!response)
                return Results.BadRequest();

            return Results.Ok();
        }

    }
}
