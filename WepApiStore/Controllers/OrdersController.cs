using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
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
            IEnumerable<OrderDto> orderDtos = _orderService.GetOrders();

            if (orderDtos == null || !orderDtos.Any())
            {
                return Ok("No orders");
            }

            return Ok(orderDtos);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            OrderDto orderDto = _orderService.GetOrder(id);

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
            OrderDto orderDto = _orderService.GetOrder(id);

            if (orderDto == null)
            {
                return NotFound();
            }

            return Ok(_orderService.GetOrderProducts(orderDto));
        }


        [HttpPost]
        public IHttpActionResult MakeOrder(OrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                _orderService.MakeOrder(orderDto);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("{id}/products")]
        public IHttpActionResult UpdateOrderProducts(int id, [FromBody]ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok("Product data is incorrect");
            }

            bool isAdded = _orderService.AddProductToOrder(id, productDto);

            if (!isAdded)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}