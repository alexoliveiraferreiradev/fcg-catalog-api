using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fcg.Catalog.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CatalogDbContext _dbContext;
        private readonly IDbConnection _dbConnection;

        public OrderRepository(CatalogDbContext dbContext, IDbConnection dbConnection)
        {
            _dbContext = dbContext;
            _dbConnection = dbConnection;
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
