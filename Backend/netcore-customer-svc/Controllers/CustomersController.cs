using netcore_customer_svc.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using netcore_customer_svc.Services;
using OpenTracing;
using System.Net.Http;

/**
GET    /api/customerss
GET    /api/customerss/customersId
POST   /api/customerss
PUT    /api/customerss/customersId
DELETE /api/customerss/customersId
*/

namespace netcore_customer_svc.Controllers
{
    [Route("api/[controller]")]
    public class customersController : Controller
    {
        private IcustomersRepository _customersRepository;
        private readonly IHttpClientFactory _factory; 
        public customersController(IcustomersRepository customersRepository, ITracer tracer) {
            _customersRepository = customersRepository;
            var operationName = "CustomerController/";
            var builder = tracer.BuildSpan(operationName);
            
            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                var log = $"Endpoint Called";
                span.Log(log + " " + span.ToString());
            }
        }

        //api/customerss
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        [ProducesResponseType(400)]
        public IActionResult Getcustomers()
        {
            try {
                var customerss = _customersRepository.Getcustomers();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(customerss);
            }
            catch (Exception e) {
                 return BadRequest();
            }
        }

        //api/customerss/customersId
        [HttpGet("{customersId}", Name = "Getcustomers")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Getcustomers(int customersId)
        {
            if (!_customersRepository.customersExists(customersId))
                return NotFound();

            var customers = _customersRepository.Getcustomer(customersId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            return Ok(customers);
        }

        //api/customerss
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Createcustomers([FromBody]Customer customersToCreate)
        {
            if (customersToCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_customersRepository.Createcustomers(customersToCreate))
            {
                ModelState.AddModelError("", "Something went wrong saving the customers ");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("Getcustomers", new { customersId = customersToCreate.customer_id }, customersToCreate);
        }

        //api/customerss/customersId
        [HttpPut("{customersId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Updatecustomers(int customersId, [FromBody]Customer customersToUpdate)
        {
            if (customersToUpdate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_customersRepository.Updatecustomers(customersToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating in the customers");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/customerss/customersId
        [HttpDelete("{customersId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Deletecustomers(int customersId)
        {
            if (!_customersRepository.customersExists(customersId))
                return NotFound();

            var customersToDelete = _customersRepository.Getcustomer(customersId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_customersRepository.Deletecustomers(customersToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting customers");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}