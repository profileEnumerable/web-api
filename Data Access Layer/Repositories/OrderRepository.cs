using Data_Access_Layer.EF;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Data_Access_Layer.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly DealContext _dbContext;

        public OrderRepository(DealContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Order> GetAll()
        {
            return _dbContext.Orders.Include(order => order.Products);
        }

        public Order Get(int id)
        {
            return _dbContext.Orders.Include("Products").FirstOrDefault(o => o.Id == id);
        }

        public void Create(Order order)
        {
            _dbContext.Orders.Add(order);
        }

        public void Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Order order = _dbContext.Orders.Find(id);

            if (order != null)
            {
                _dbContext.Orders.Remove(order);
            }
        }
    }
}
