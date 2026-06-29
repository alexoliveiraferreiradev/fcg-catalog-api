using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Pedidos.Query.ObtemHistoricoPedido
{
    public record ObtemHistoricoPedidoQuery(Guid UsuarioId) : IRequest<IEnumerable<PedidoHistoricoResponse>>;
}
