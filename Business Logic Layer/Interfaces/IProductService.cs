using System.Collections.Generic;
using Business_Logic_Layer.DTO;

namespace Business_Logic_Layer.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductDTO> GetProducts();
        void AddProduct(ProductDTO productDto);
        void Dispose();
    }
}