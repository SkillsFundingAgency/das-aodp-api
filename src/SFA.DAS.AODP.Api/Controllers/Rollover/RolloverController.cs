using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Models.Rollover;

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
        return await SendRequestAsync(query);
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
    [ProducesResponseType(typeof(CreateRolloverWorkflowRunCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRolloverWorkflowRun(CreateRolloverWorkflowRunCommand createRolloverWorkflowRunCommand)
    {
        return await SendRequestAsync(createRolloverWorkflowRunCommand);
    }

    [HttpPost("removepreviousworkflowcandidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemovePreviousWorkflowCandidates()
    {
        var command = new RemovePreviousWorkflowCandidatesCommand();
        return await SendRequestAsync(command);
    }

    [HttpGet("{rolloverWorkflowRunId}/rollovercandidatesforexport")]
    [ProducesResponseType(typeof(GetRolloverCandidatesForExportQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverCandidatesForExport(Guid rolloverWorkflowRunId, CancellationToken cancellationToken)
    {
        var query = new GetRolloverCandidatesForExportQuery { RolloverWorkflowRunId = rolloverWorkflowRunId};
        return await SendRequestAsync(query);
    }

    [HttpPost("validaterolloverextension")]
    [ProducesResponseType(typeof(ValidateRolloverExtensionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateRolloverExtension(ValidateRolloverExtensionCommand validateRolloverExtensionCommand)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return await SendRequestAsync(validateRolloverExtensionCommand);
    }

    [HttpPost("submitrolloverextension")]
    [ProducesResponseType(typeof(SubmitRolloverExtensionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitRolloverExtension(SubmitRolloverExtensionCommand submitRolloverExtensinCommand)
    {
        return await SendRequestAsync(submitRolloverExtensinCommand);
    }

    [HttpPost("querybuilder/awardingorganisations")]
    [ProducesResponseType(typeof(GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAwardingOrganisationsForRolloverQueryBuilder(
        [FromBody] RolloverQueryBuilderAwardingOrganisationsRequest filters)
    {
        return await SendRequestAsync(new GetAwardingOrganisationsForRolloverQueryBuilderQuery(filters));
    }

    [HttpPost("querybuilder/qualificationversions")]
    [ProducesResponseType(typeof(GetQualificationVersionsForRolloverQueryBuilderQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQualificationVersionsForRolloverQueryBuilder(
        [FromBody] RolloverQueryBuilderRequest filters)
    {
        return await SendRequestAsync(new GetQualificationVersionsForRolloverQueryBuilderQuery(filters));
    }

    [HttpGet("querybuilder/levels")]
    [ProducesResponseType(typeof(GetLevelsForRolloverQueryBuilderQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLevelsForRolloverQueryBuilder()
    {
        return await SendRequestAsync(new GetLevelsForRolloverQueryBuilderQuery());
    }

    [HttpPost("querybuilder/types")]
    [ProducesResponseType(typeof(GetTypesForRolloverQueryBuilderQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTypesForRolloverQueryBuilder([FromBody] RolloverQueryBuilderTypesRequest filters)
    {
        return await SendRequestAsync(new GetTypesForRolloverQueryBuilderQuery(filters));
    }

    [HttpPost("querybuilder/sectorsubjectarea")]
    [ProducesResponseType(typeof(GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSectorSubjectAreasForRolloverQueryBuilder([FromBody] RolloverQueryBuilderSectorSubjectAreaRequest filters)
    {
        return await SendRequestAsync(new GetSectorSubjectAreasForRolloverQueryBuilderQuery(filters));
    }

    [HttpGet("startsummary")]
    [ProducesResponseType(typeof(GetRolloverStartSummaryQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRolloverStartSummary()
    {
        return await SendRequestAsync(new GetRolloverStartSummaryQuery());
    }
}
