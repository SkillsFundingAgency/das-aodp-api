using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Queries.Rollover;

namespace SFA.DAS.AODP.Api.Controllers.Rollover;

[ApiController]
[Route("api/rollover")]
public class RolloverController : BaseController
{

    public RolloverController(IMediator mediator, ILogger<RolloverController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("workflowcandidates")]
    [ProducesResponseType(typeof(GetRolloverWorkflowCandidatesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverWorkflowCandidates([FromQuery] int? skip, [FromQuery] int? take)
    {
        var query = new GetRolloverWorkflowCandidatesQuery(skip, take);
        return await SendRequestAsync(query);
    }
}