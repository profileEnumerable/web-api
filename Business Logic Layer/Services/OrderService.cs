using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business_Logic_Layer.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork DataBase { get; }

        public OrderService(IUnitOfWork dataBase)
        {
            DataBase = dataBase;
        }

        public IEnumerable<OrderDto> GetOrders()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDto>()).CreateMapper();
            return mapper.Map<IEnumerable<Order>, IEnumerable<OrderDto>>(DataBase.Orders.GetAll());
        }

        public void MakeOrder(OrderDto orderDto)
        {
            var order = new Order
            {
                Name = orderDto.Name,
                Products = orderDto.Products
            };

            DataBase.Orders.Create(order);
            DataBase.Save();
        }

        public OrderDto GetOrder(int id)
        {
            Order order = DataBase.Orders.Get(id);

            if (order == null)
            {
                return null;
            }

            return new OrderDto() { Id = id, Name = order.Name, Products = order.Products };
        }

        public IEnumerable<ProductDto> GetOrderProducts(OrderDto orderDto)
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDto>()).CreateMapper();
            return mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(orderDto.Products);
        }

        public bool AddProductToOrder(int id, ProductDto productDto)
        {
            Order order = DataBase.Orders.Get(id);

            if (order == null)
            {
                return false;
            }

            var newProduct = new Product()
            {
                Name = productDto.Name,
                Manufacturer = productDto.Manufacturer,
                YearOfIssue = productDto.YearOfIssue
            };

            order.Products.Add(newProduct);
            DataBase.Save();

            return true;
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}