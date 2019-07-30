using Data_Access_Layer.EF;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System;

namespace Data_Access_Layer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DealContext _dbContext;
        private OrderRepository _orderRepository;
        private ProductRepository _productRepository;

        public UnitOfWork(string connectionString)
        {
            _dbContext = new DealContext(connectionString);
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(_dbContext);
                }

                return _orderRepository;
            }
        }

        public IRepository<Product> Products
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_dbContext);
                }

                return _productRepository;
            }

        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        private bool disposed = false;


        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
