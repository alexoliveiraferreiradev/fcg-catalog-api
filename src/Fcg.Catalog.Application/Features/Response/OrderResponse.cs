using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> InformativeMessages { get; set; } = new();
        public IEnumerable<OrderItemResponse> Items { get; set; }
        public OrderResponse()
        {

        }
    }
}
