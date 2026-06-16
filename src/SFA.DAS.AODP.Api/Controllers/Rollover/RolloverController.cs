using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Queries.Rollover;

namespace SFA.DAS.AODP.Api.Controllers.Rollover;

[ApiController]
[Route("api/rollover")]
public class RolloverController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<RolloverController> _logger;

    public RolloverController(IMediator mediator, ILogger<RolloverController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("workflowcandidatescount")]
    [ProducesResponseType(typeof(GetRolloverWorkflowCandidatesCountQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverWorkflowCandidatesCount(CancellationToken cancellationToken)
    {
        var query = new GetRolloverWorkflowCandidatesCountQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("p1checks")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RolloverWorkflowCandidatesAfterP1Checks([FromBody] UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand command)
    {
        return await SendRequestAsync(command);
    }

    [HttpGet("rollovercandidates")]
    [ProducesResponseType(typeof(GetRolloverCandidatesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverCandidates()
    {
        var query = new GetRolloverCandidatesQuery();
        return await SendRequestAsync(query);
    }

    [HttpGet("rolloverworkflowcandidates")]
    [ProducesResponseType(typeof(GetRolloverWorkflowCandidatesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverWorkflowCandidates()
    {
        var query = new GetRolloverWorkflowCandidatesQuery();
        return await SendRequestAsync(query);
    }

    [HttpPost("rolloverworkflowruns")]
    [ProducesResponseType(typeof(BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRolloverWorkflowRun(CreateRolloverWorkflowRunCommand createRolloverWorkflowRunCommand)
    {
        return await SendRequestAsync(createRolloverWorkflowRunCommand);
    }

    [HttpGet("{rolloverWorkflowRunId}/rollovercandidatesforexport")]
    [ProducesResponseType(typeof(GetRolloverCandidatesForExportQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverCandidatesForExport(Guid rolloverWorkflowRunId, CancellationToken cancellationToken)
    {
        var query = new GetRolloverCandidatesForExportQuery { RolloverWorkflowRunId = rolloverWorkflowRunId};
        return await SendRequestAsync(query);
    }

    [HttpPost("rolloverextensionvalidation")]
    [ProducesResponseType(typeof(BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateFundingExtensionCandidatesCommand(ValidateFundingExtensionCandidatesCommand validateFundingExtensionCandidatesCommand)
    {
        return await SendRequestAsync(validateFundingExtensionCandidatesCommand);
    }

    [HttpPost("applyrolloverextension")]
    [ProducesResponseType(typeof(BaseMediatrResponse<ApplyFundingExtensionsCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApplyFundingExtensionsToCandidates(ApplyFundingExtensionsCommand applyFundingExtensionToCandidatesCommand)
    {
        return await SendRequestAsync(applyFundingExtensionToCandidatesCommand);
    }
}