using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;

[ApiController]
[Route("[controller]")]
public class PingController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PingController> _logger;

    public PingController(IMediator mediator, ILogger<PingController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet("/Ping")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Ping()
    {
        //var query = new Ping();
        //return await SendRequestAsync(query);

        return Ok("Pong");
    }
}

