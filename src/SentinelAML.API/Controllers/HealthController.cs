using Microsoft.AspNetCore.Mvc;

namespace SentinelAML.API.Controllers;

public class HealthController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "healthy", service = "SentinelAML.API" });
}
