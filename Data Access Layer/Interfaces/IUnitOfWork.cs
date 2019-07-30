using Data_Access_Layer.Entities;
using System;

namespace Data_Access_Layer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Order> Orders { get; }
        IRepository<Product> Products { get; }
        void Save();
    }
}