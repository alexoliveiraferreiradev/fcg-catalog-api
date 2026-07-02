using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class PedidoHistoricoResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DataPedido { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PedidoItemResponse> Items { get; set; } = new();
        public PedidoHistoricoResponse()
        {

        }
    }
}
