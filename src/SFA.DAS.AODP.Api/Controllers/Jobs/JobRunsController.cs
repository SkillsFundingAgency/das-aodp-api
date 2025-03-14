using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Queries.Jobs;

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

    [HttpGet("/api/job-runs")]
    [ProducesResponseType(typeof(GetJobRunsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new GetJobRunsQuery();
        return await SendRequestAsync(query);
    }
}

