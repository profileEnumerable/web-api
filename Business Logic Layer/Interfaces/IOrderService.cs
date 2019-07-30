using System.Collections.Generic;
using Business_Logic_Layer.DTO;

namespace Business_Logic_Layer.Interfaces
{
    public interface IOrderService
    {
        void MakeOrder(OrderDTO orderDto);
        IEnumerable<OrderDTO> GetOrders();
        OrderDTO GetOrder(int id);
        IEnumerable<ProductDTO> GetOrderProducts(int id);
        void AddProductToOrder(int id,ProductDTO productDto);
    }
}
