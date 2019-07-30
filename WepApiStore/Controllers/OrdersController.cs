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
    }
}
