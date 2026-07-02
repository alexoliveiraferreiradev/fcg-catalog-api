using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Query.GetOrderHistory
{
    public record ObtemHistoricoPedidoQuery(Guid UserId) : IRequest<IEnumerable<PedidoHistoricoResponse>>;
}
