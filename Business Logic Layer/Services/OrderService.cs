using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System;
using System.Collections.Generic;

namespace Business_Logic_Layer.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork DataBase { get; }

        public OrderService(IUnitOfWork dataBase)
        {
            DataBase = dataBase;
        }

        public IEnumerable<OrderDTO> GetOrders()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(DataBase.Orders.GetAll());
        }

        //public void MakeOrder(OrderDTO orderDto)
        //{
        //    ICollection<Product> products = orderDto.Products;

        //    if (products == null || products.Count == 0)
        //    {
        //        throw new Exception("No products selected");
        //    }

        //    var order = new Order
        //    {
        //        Name = orderDto.Name,
        //        Products = products
        //    };

        //    DataBase.Orders.Create(order);
        //    DataBase.Save();
        //}

        public OrderDTO GetOrder(int id)
        {
            Order order = DataBase.Orders.Get(id);

            if (order == null)
            {
                return null;
            }

            return new OrderDTO() { Id = id, Name = order.Name, Products = order.Products };
        }

        public IEnumerable<ProductDTO> GetOrderProducts(OrderDTO orderDto)
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<Product,ProductDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(orderDto.Products);
        }

        public void AddProductToOrder(int id, ProductDTO productDto)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}