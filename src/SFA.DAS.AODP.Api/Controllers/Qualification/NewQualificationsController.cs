using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;

namespace SFA.DAS.AODP.Api.Controllers.Qualification
{
    [ApiController]
    [Route("api/new-qualifications")]
    public class NewQualificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NewQualificationsController> _logger;

        public NewQualificationsController(IMediator mediator, ILogger<NewQualificationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNewQualifications()
        {
            _logger.LogInformation("Getting all new qualifications");

            var result = await _mediator.Send(new GetNewQualificationsQuery());

            if (!result.Success)
            {
                _logger.LogWarning("No new qualifications found");
                return NotFound(result);
            }

            _logger.LogInformation("Successfully retrieved new qualifications");
            return Ok(result);
        }

        [HttpGet("{qualificationReference}")]
        public async Task<IActionResult> GetQualificationDetails(string qualificationReference)
        {
            if (string.IsNullOrWhiteSpace(qualificationReference))
            {
                _logger.LogWarning("Qualification reference is empty");
                var response = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>
                {
                    Success = false,
                    ErrorMessage = "Qualification reference cannot be empty"
                };
                return BadRequest(response);
            }

            _logger.LogInformation("Getting details for qualification reference: {QualificationReference}", qualificationReference);

            var result = await _mediator.Send(new GetQualificationDetailsQuery { QualificationReference = qualificationReference });

            if (!result.Success)
            {
                _logger.LogWarning("No details found for qualification reference: {QualificationReference}", qualificationReference);
                return NotFound(result);
            }

            _logger.LogInformation("Successfully retrieved details for qualification reference: {QualificationReference}", qualificationReference);
            return Ok(result);
        }

    }
}

