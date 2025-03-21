using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Enum;

namespace SFA.DAS.AODP.Api.Controllers.Jobs;

[ApiController]
[Route("api/[controller]")]
public class JobRunsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobRunsController> _logger;

    public JobRunsController(IMediator mediator, ILogger<JobRunsController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/job/{jobName}/runs")]
    [ProducesResponseType(typeof(GetJobRunsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync(string jobName)
    {
        if (string.IsNullOrWhiteSpace(jobName))
        {
            _logger.LogWarning("Job name is empty");
            return BadRequest(new { message = "Job name cannot be empty" });
        }

        var query = new GetJobRunsQuery(jobName);
        return await SendRequestAsync(query);
    }

    [HttpPost("/api/job/requestrun")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] RequestJobRunCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.JobName))
        {
            _logger.LogWarning("Job name is empty");
            return BadRequest(new { message = "Job name cannot be empty" });
        }

        return await SendRequestAsync(command);
    }
}

