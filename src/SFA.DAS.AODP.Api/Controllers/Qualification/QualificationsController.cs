using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Api.Controllers.Qualification
{
    [ApiController]
    [Route("api/qualifications")]
    public class QualificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<QualificationsController> _logger;

        public QualificationsController(IMediator mediator, ILogger<QualificationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseMediatrResponse<GetNewQualificationsQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQualifications([FromQuery] string? status)
        {
            var processedStatus = ProcessAndValidateStatus(status);
            if (processedStatus is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            IActionResult response = processedStatus switch
            {
                "new" => await HandleNewQualifications(),
                // Add more cases for other statuses
                _ => BadRequest(new { message = $"Invalid status: {processedStatus}" })
            };

            return response;
        }

        [HttpGet("{qualificationReference}")]
        [ProducesResponseType(typeof(BaseMediatrResponse<GetQualificationDetailsQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQualificationDetails(string? qualificationReference)
        {
            if (string.IsNullOrWhiteSpace(qualificationReference))
            {
                _logger.LogWarning("Qualification reference is empty");
                return BadRequest(new { message = "Qualification reference cannot be empty" });
            }

            var result = await _mediator.Send(new GetQualificationDetailsQuery { QualificationReference = qualificationReference });

            if (!result.Success || result.Value == null)
            {
                _logger.LogWarning(result.ErrorMessage);
                return NotFound(new { message = $"No details found for qualification reference: {qualificationReference}" });
            }

            return Ok(result);
        }

        [HttpGet("qualifications/export")]
        [ProducesResponseType(typeof(BaseMediatrResponse<List<QualificationExport>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQualificationCSVExportData([FromQuery] string? status)
        {
            var processedStatus = ProcessAndValidateStatus(status);
            if (processedStatus is IActionResult badRequestResult)
            {
                return badRequestResult;
            }

            IActionResult response = processedStatus switch
            {
                "new" => await HandleNewQualificationCSVExport(),
                // Add more cases for other statuses
                _ => BadRequest(new { message = $"Invalid status: {processedStatus}" })
            };

            return response;
        }

        private async Task<IActionResult> HandleNewQualifications()
        {
            var result = await _mediator.Send(new GetNewQualificationsQuery());

            if (result == null || !result.Success || result.Value == null)
            {
                _logger.LogWarning("No new qualifications found.");
                return NotFound(new { message = "No new qualifications found" });
            }

            return Ok(result);
        }

        private async Task<IActionResult> HandleNewQualificationCSVExport()
        {
            var result = await _mediator.Send(new GetNewQualificationsCSVExportQuery());

            if (result == null || !result.Success || result.Value == null)
            {
                _logger.LogWarning("No new qualification data found for export.");
                return NotFound(new { message = "No new qualification data found for export" });
            }

            return Ok(result);
        }

        private object ProcessAndValidateStatus(string status)
        {
            status = status?.Trim().ToLower();

            if (string.IsNullOrEmpty(status))
            {
                _logger.LogWarning("Qualification status is missing.");
                return BadRequest(new { message = "Qualification status cannot be empty." });
            }

            return status;
        }
    }
}



