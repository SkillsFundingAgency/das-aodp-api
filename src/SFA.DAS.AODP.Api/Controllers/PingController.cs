using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController: Controller
{
    private readonly ILogger<PingController> _logger;

    public PingController(ILogger<PingController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet("/Ping")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}

