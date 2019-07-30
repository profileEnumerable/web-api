using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Interfaces;
using System.Collections.Generic;
using System.Web.Http;

namespace WepApiStore.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            IEnumerable<ProductDTO> productDtos = _productService.GetProducts();

            if (productDtos == null)
            {
                return NotFound();
            }

            return Ok(productDtos);
        }

        [HttpPost]
        public IHttpActionResult Add([FromBody]ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok("Product data isn't valid");
            }

            _productService.AddProduct(productDto);

            return Ok();
        }
    }
}
