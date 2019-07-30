using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using System;
using System.Collections.Generic;

namespace Business_Logic_Layer.Services
{
    public class ProductService : IProductService
    {
        private IUnitOfWork DataBase { get; }

        public ProductService(IUnitOfWork dataBase)
        {
            DataBase = dataBase;
        }

        public IEnumerable<ProductDTO> GetProducts()
        {
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>()).CreateMapper();

            return mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(DataBase.Products.GetAll());
        }

        public void AddProduct(ProductDTO productDto)
        {
            var newProduct = new Product()
            {
                Name = productDto.Name,
                Manufacturer = productDto.Manufacturer,
                YearOfIssue = productDto.YearOfIssue
            };

            DataBase.Products.Create(newProduct);
            DataBase.Save();
        }

        public void Dispose()
        {
            DataBase.Dispose();
        }
    }
}