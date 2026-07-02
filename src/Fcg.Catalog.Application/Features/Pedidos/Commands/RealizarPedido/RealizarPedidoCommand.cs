using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Pedidos.Commands.RealizarPedido
{
    public record RealizarPedidoCommand(Guid UsuarioId, string NomeUsuario,string EmailUsuario, IEnumerable<Guid> JogosIds) : IRequest<bool>;
}
