using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Box.Controllers;

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
    
    // GET: api/box/{id}
    [HttpGet("{id}")]
    public ActionResult<Entities.Box> GetBox(int id)
    {
        // Get the box corresponding to the id
        return Ok(new Entities.Box(10));
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

