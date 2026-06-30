using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Pedidos.Commands.RealizarPedido
{
    public record RealizarPedidoCommand(Guid UsuarioId, string NomeUsuario,string EmailUsuario, IEnumerable<Guid> JogosIds) : IRequest<bool>;
}
