using Data_Access_Layer.EF;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;

namespace Data_Access_Layer.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly DealContext _dbContext;

        public ProductRepository(DealContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetAll()
        {
            return _dbContext.Products;
        }

        public Product Get(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public void Create(Product item)
        {
            _dbContext.Products.Add(item);
        }

        public void Update(Product item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Product product = _dbContext.Products.Find(id);

            if (product != null)
            {
                _dbContext.Products.Remove(product);
            }
        }
    }
}