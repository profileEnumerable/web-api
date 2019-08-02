using System.Collections.Generic;
using Business_Logic_Layer.DTO;

namespace Business_Logic_Layer.Interfaces
{
    public interface IOrderService
    {
        void MakeOrder(OrderDto orderDto);
        IEnumerable<OrderDto> GetOrders();
        OrderDto GetOrder(int id);
        IEnumerable<ProductDto> GetOrderProducts(OrderDto orderDto);
        bool AddProductToOrder(int id,ProductDto productDto);
        void Dispose();
    }
}
