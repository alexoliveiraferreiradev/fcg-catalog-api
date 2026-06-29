using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoCommandHandler : IRequestHandler<RealizarPedidoCommand, PedidoResponse>
    {
        public Task<PedidoResponse> Handle(RealizarPedidoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
