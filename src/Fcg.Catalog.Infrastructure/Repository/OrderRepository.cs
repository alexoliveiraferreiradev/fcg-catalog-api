using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalog.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CatalogDbContext _dbContext;

        public OrderRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Order order)
        {
           _dbContext.Orders.Add(order);    
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _dbContext.Orders.Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public void Update(Order order)
        {
           _dbContext.Orders.Update(order); 
        }
    }
}
