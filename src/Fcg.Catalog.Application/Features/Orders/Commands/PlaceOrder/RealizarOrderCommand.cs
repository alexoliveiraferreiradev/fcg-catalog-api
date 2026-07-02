using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder
{
    public record RealizarPedidoCommand(Guid UserId, string NomeUsuario,string EmailUsuario, IEnumerable<Guid> JogosIds) : IRequest<bool>;
}
