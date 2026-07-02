using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class PedidoResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DataPedido { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> MensagensInformativas { get; set; } = new();
        public IEnumerable<PedidoItemResponse> Items { get; set; }
        public PedidoResponse()
        {

        }
    }
}
