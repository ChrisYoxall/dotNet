using Microsoft.AspNetCore.Mvc;


namespace Box.Controllers
{
    [Route("api/box")]
    [ApiController]
    public class BoxController : ControllerBase
    {
        private readonly ILogger _logger;

        public BoxController(ILogger<BoxController> logger)
        {
            _logger = logger;
        }
        
        // See controller action return types at https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types
        
        // GET: api/box
        [HttpGet]
        public ActionResult<string []> Get()
        {
            _logger.LogInformation("In the GET api/box endpoint.");
            return Ok(new string [] {"value1", "value2"});
        }
        
        // GET: api/box/getbox
        [HttpGet("/api/box/getBox")]
        public ActionResult<Box.Entities.Box> GetBox()
        {
            return Ok(new Box.Entities.Box(10));
        }
        
        // GET: api/box/asyncProcess
        [HttpGet("/api/box/processAsync")]
        public ActionResult<Box.Entities.Box> ProcessAsync()
        {
            // Start the asynchronous task in the background
            Task.Run(PerformLongRunningOperation);

            // Return a success response immediately
            return Ok();
        }
        
        private async Task PerformLongRunningOperation()
        {
            // Simulate a long-running operation
            await Task.Delay(5000); // Wait for 5 seconds

            // Perform the actual long-running operation here
            _logger.LogInformation("Long-running operation completed.");
        }

        // GET api/box/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/box
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/box/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/box/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
