using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Query.ObtemHistoricoPedido
{
    public record ObtemHistoricoPedidoQuery(Guid UserId) : IRequest<IEnumerable<PedidoHistoricoResponse>>;
}
