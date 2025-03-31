using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Application.Commands.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Application.Commands;
using System.Linq;

namespace SFA.DAS.AODP.Api.Controllers.Qualification;
[ApiController]
[Route("api/qualifications")]
public class QualificationsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<QualificationsController> _logger;

    public QualificationsController(IMediator mediator, ILogger<QualificationsController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/qualifications/{qualificationReference}/QualificationVersions")]
    [ProducesResponseType(typeof(GetQualificationVersionsForQualificationByReferenceQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQualificationVersionsForQualificationByReference(string qualificationReference)
    {
        return await SendRequestAsync(new GetQualificationVersionsForQualificationByReferenceQuery(qualificationReference));
    }

    [HttpGet("/api/qualifications/{qualificationVersionId}/feedback")]
    [ProducesResponseType(typeof(GetFeedbackForQualificationFundingByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFeedbackForQualificationFundingById(Guid qualificationVersionId)
    {
        return await SendRequestAsync(new GetFeedbackForQualificationFundingByIdQuery(qualificationVersionId));
    }

    [HttpPut("/api/qualifications/{qualificationVersionId}/save-qualification-funding-offers-outcome")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveFundingOfferOutcome(SaveQualificationsFundingOffersOutcomeCommand command, Guid qualificationVersionId)
    {
        command.QualificationVersionId = qualificationVersionId;
        return await SendRequestAsync(command);
    }

    [HttpPut("/api/qualifications/{qualificationVersionId}/save-qualification-funding-offers")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveQualificationFundingOffers(SaveQualificationsFundingOffersCommand command, Guid qualificationVersionId)
    {
        command.QualificationVersionId = qualificationVersionId;

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/qualifications/{qualificationVersionId}/save-qualification-funding-offers-details")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveQualificationFundingOffersDetails(SaveQualificationsFundingOffersDetailsCommand command, Guid qualificationVersionId)
    {
        command.QualificationVersionId = qualificationVersionId;

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/qualifications/{qualificationVersionId}/Create-QualificationDiscussionHistory")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> QualificationFundingOffersSummary(CreateQualificationDiscussionHistoryCommand command, Guid qualificationVersionId)
    {
        return await SendRequestAsync(command);
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseMediatrResponse<GetNewQualificationsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseMediatrResponse<GetChangedQualificationsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQualifications([FromQuery] string? status,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromQuery] string? name,
        [FromQuery] string? organisation,
        [FromQuery] string? qan)
    {
        var validationResult = ValidateQualificationParams(status, skip, take, name, organisation, qan);

        if (validationResult.IsValid)
        {
            if (validationResult.ProcessedStatus == "new")
            {
                var query = new GetNewQualificationsQuery()
                {
                    Name = name,
                    Organisation = organisation,
                    QAN = qan,
                    Skip = skip,
                    Take = take
                };

                return await SendRequestAsync(query);
            }
            else if (validationResult.ProcessedStatus == "changed")
            {
                var query = new GetChangedQualificationsQuery()
                {
                    Name = name,
                    Organisation = organisation,
                    QAN = qan,
                    Skip = skip,
                    Take = take
                };
                return await SendRequestAsync(query);
            }
            else
            {
                return BadRequest(new { message = $"status '{status}' parameter is not valid" });
            }
        }
        else
        {
            return BadRequest(new { message = validationResult.ErrorMessage });
        }

    }

    [HttpGet("{qualificationReference}/detail")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQualificationDetails(string? qualificationReference)
    {
        if (string.IsNullOrWhiteSpace(qualificationReference))
        {
            _logger.LogWarning("Qualification reference is empty");
            return BadRequest(new { message = "Qualification reference cannot be empty" });
        }
        var query = new GetQualificationDetailsQuery()
        {
            QualificationReference = qualificationReference
        };
        return await SendRequestAsync(query);
    }

    [HttpGet("{qualificationReference}/qualificationversions/{version}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQualificationDetails(string? qualificationReference, int version)
    {
        if (string.IsNullOrWhiteSpace(qualificationReference))
        {
            _logger.LogWarning("Qualification reference is empty");
            return BadRequest(new { message = "Qualification reference cannot be empty" });
        }
        var query = new GetQualificationVersionQuery()
        {
            QualificationReference = qualificationReference,
            Version = version

        };
        return await SendRequestAsync(query);
    }

    [HttpPost("qualificationdiscussionhistory")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddQualification([FromBody] AddQualificationDiscussionHistoryCommand qualificationDiscussionHistory)
    {
        return await SendRequestAsync(qualificationDiscussionHistory);
    }

    [HttpGet("processstatuses")]
    [ProducesResponseType(typeof(GetProcessingStatusesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProcessingStatuses()
    {
        return await SendRequestAsync(new GetProcessingStatusesQuery());
    }

    [HttpPost("qualificationstatus")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateQualificationStatus([FromBody] UpdateQualificationStatusCommand qualificationStatus)
    {
        return await SendRequestAsync(qualificationStatus);
    }

    [HttpGet("{qualificationReference}/qualificationdiscussionhistories")]
    [ProducesResponseType(typeof(BaseMediatrResponse<GetDiscussionHistoriesForQualificationQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDiscussionHistoriesForQualification(string qualificationReference)
    {
        if (string.IsNullOrWhiteSpace(qualificationReference))
        {
            _logger.LogWarning("Qualification reference is empty");
            return BadRequest(new { message = "Qualification reference cannot be empty" });
        }
        return await SendRequestAsync(new GetDiscussionHistoriesForQualificationQuery { QualificationReference = qualificationReference });
    }

    [HttpGet("export")]
    [ProducesResponseType(typeof(BaseMediatrResponse<GetNewQualificationsCsvExportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseMediatrResponse<GetChangedQualificationsCsvExportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQualificationCSVExportData([FromQuery] string? status)
    {
        IActionResult response = status?.ToLower() switch
        {
            "new" => await HandleNewQualificationCSVExport(),
            "changed" => await HandleChangedQualificationCSVExport(),
            _ => BadRequest(new { message = $"Invalid status param: {status}" })
        };

        return response;
    }

    private async Task<IActionResult> HandleNewQualificationCSVExport()
    {
        var result = await _mediator.Send(new GetNewQualificationsCsvExportQuery());

        if (result == null || !result.Success || result.Value == null)
        {
            _logger.LogWarning(result.ErrorMessage);
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    [HttpGet("GetActionTypes")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActionTypes()
    {
        var result = await _mediator.Send(new GetActionTypesQuery());

        if (result == null || !result.Success || result.Value == null)
        {
            _logger.LogWarning(result.ErrorMessage);
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }
    private async Task<IActionResult> HandleChangedQualificationCSVExport()
    {
        var result = await _mediator.Send(new GetChangedQualificationsCsvExportQuery());

        if (result == null || !result.Success || result.Value == null)
        {
            _logger.LogWarning(result.ErrorMessage);
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }


    private ParamValidationResult ValidateQualificationParams(string? status, int? skip, int? take, string? name, string? organisation, string? qan)
    {
        var result = new ParamValidationResult() { IsValid = true };
        status = status?.Trim().ToLower();

        if (string.IsNullOrEmpty(status))
        {
            result.IsValid = false;
            result.ErrorMessage = "Qualification status cannot be empty.";
        }
        else
        {
            result.ProcessedStatus = status;
        }

        if (skip < 0)
        {
            result.IsValid = false;
            result.ErrorMessage = "Skip param is invalid.";
        }

        if (take < 0)
        {
            result.IsValid = false;
            result.ErrorMessage = "Take param is invalid.";
        }

        if (!result.IsValid)
        {
            _logger.LogWarning(result.ErrorMessage);
        }

        return result;
    }

    private class ParamValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ProcessedStatus { get; set; }
    }

}
