using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.Jobs;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class JobsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IMediator mediator, ILogger<JobsController> logger) : base(mediator, logger)
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
        return await SendRequestAsync(query);
    }


    [HttpGet("/api/job")]
    [ProducesResponseType(typeof(GetJobByNameQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByNameAsync([FromQuery] string name)
    {        
        var query = new GetJobByNameQuery(name);
        return await SendRequestAsync(query);
    }

    [HttpPut("/api/job")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateJobCommand command)
    {
        return await SendRequestAsync(command);
    }
}
