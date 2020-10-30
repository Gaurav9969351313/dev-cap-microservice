using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace logging_svc.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IHttpClientFactory _factory; 

        public ValuesController(ILogger<ValuesController> logger,  ITracer tracer)
        {
            _logger = logger;
            var operationName = "LoggingController/";
            var builder = tracer.BuildSpan(operationName);
            
            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                var log = $"Endpoint Called";
                span.Log(log + " " + span.ToString());
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _logger.LogInformation("Info logging");
            _logger.LogCritical("Critical logging");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
