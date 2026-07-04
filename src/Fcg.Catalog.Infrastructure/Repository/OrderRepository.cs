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
            return await _dbContext.Orders.Include(o=>o.Games)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public void Update(Order order)
        {
           _dbContext.Orders.Update(order); 
        }
    }
}
