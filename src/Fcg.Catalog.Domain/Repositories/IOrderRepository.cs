using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        void Update(Order order);
        Task<Order> GetOrderById(Guid orderId);
    }
}
