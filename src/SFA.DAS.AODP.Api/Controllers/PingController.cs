using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AODP.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : Controller
    {
        private readonly IConfiguration configuration;

        public PingController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("/test")]
        public IActionResult Ping()
        {
            var configValue = configuration["Test"];
            if (configValue == null)
            {
                return BadRequest("Config not found");
            }
            else if(configValue != "123")
            {
                return BadRequest("Config not expected value");

            }
            return Ok("Pong");
        }
    }
}
