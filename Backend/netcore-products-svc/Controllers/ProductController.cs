using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using netcore_products_svc.Models;
using netcore_products_svc.Services;
using OpenTracing;

/**

GET    /api/productss
GET    /api/productss/productsId
POST   /api/productss
PUT    /api/productss/productsId
DELETE /api/productss/productsId
*/
namespace netcore_products_svc.Controllers
{
    [Route("api/[controller]")]
    public class productsController : Controller
    {
        private IProductsRepository _productsRepository;

        public productsController(IProductsRepository productsRepository, ITracer tracer) {
            _productsRepository = productsRepository;

            var operationName = "ProductsController/";
            var builder = tracer.BuildSpan(operationName);
            
            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                var log = $"Endpoint Called ";
                span.Log(log + " " + span.ToString());
            }
        }

        //api/productss
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(400)]
        public IActionResult Getproducts()
        {
            try {
                var productss = _productsRepository.Getproducts();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(productss);
            }
            catch (Exception e) {
                 return BadRequest();
            }
        }

        //api/productss/productsId
        [HttpGet("{productsId}", Name = "Getproducts")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Getproducts(int productsId)
        {
            if (!_productsRepository.productsExists(productsId))
                return NotFound();

            var products = _productsRepository.Getproduct(productsId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            return Ok(products);
        }

        //api/productss
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Createproducts([FromBody]Product productsToCreate)
        {
            if (productsToCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productsRepository.Createproducts(productsToCreate))
            {
                ModelState.AddModelError("", "Something went wrong saving the products ");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("Getproducts", new { productsId = productsToCreate.product_id }, productsToCreate);
        }

        //api/productss/productsId
        [HttpPut("{productsId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Updateproducts(int productsId, [FromBody]Product productsToUpdate)
        {
            if (productsToUpdate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productsRepository.Updateproducts(productsToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating in the products");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/productss/productsId
        [HttpDelete("{productsId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Deleteproducts(int productsId)
        {
            if (!_productsRepository.productsExists(productsId))
                return NotFound();

            var productsToDelete = _productsRepository.Getproduct(productsId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productsRepository.Deleteproducts(productsToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting products");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}