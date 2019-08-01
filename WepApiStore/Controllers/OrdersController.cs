using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WepApiStore.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            IEnumerable<OrderDTO> orderDtos = _orderService.GetOrders();

            if (orderDtos == null || !orderDtos.Any())
            {
                return Ok("No orders");
            }

            return Ok(orderDtos);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            OrderDTO orderDto = _orderService.GetOrder(id);

            if (orderDto == null)
            {
                return NotFound();
            }

            return Ok(orderDto);
        }

        [HttpGet]
        [Route("{id}/products")]
        public IHttpActionResult GetProductsByOrderId(int id)
        {
            OrderDTO orderDto = _orderService.GetOrder(id);

            if (orderDto == null)
            {
                return NotFound();
            }

            return Ok(_orderService.GetOrderProducts(orderDto));
        }
    }
}