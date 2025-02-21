using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.Jobs;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class JobsController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IMediator mediator, ILogger<JobsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
   
    
    [HttpGet("/api/jobs")]
    [ProducesResponseType(typeof(GetJobsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new GetJobsQuery();
        var response = await _mediator.Send(query);
        if (response.Success)
        {
            return Ok(response.Value);
        }

        _logger.LogError(message: $"Error thrown getting jobs.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }


    [HttpGet("/api/job")]
    [ProducesResponseType(typeof(GetJobByNameQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByNameAsync([FromQuery] string name)
    {        
        var query = new GetJobByNameQuery(name);

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response.Value);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request for job with name `{name}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting with name `{name}.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }   

}
