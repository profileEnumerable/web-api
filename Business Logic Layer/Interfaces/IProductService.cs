using System.Collections.Generic;
using Business_Logic_Layer.DTO;

namespace Business_Logic_Layer.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductDto> GetProducts();
        void AddProduct(ProductDto productDto);
        void Dispose();
    }
}