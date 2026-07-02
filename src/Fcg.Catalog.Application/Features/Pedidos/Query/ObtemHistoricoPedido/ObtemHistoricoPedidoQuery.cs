using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Pedidos.Query.ObtemHistoricoPedido
{
    public record ObtemHistoricoPedidoQuery(Guid UsuarioId) : IRequest<IEnumerable<PedidoHistoricoResponse>>;
}
