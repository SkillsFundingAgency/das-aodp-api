using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Queries.QaaQualification;

namespace SFA.DAS.AODP.Api.Controllers.QaaQualification;

[ApiController]
[Route("api/qaa")]
public class QaaController : BaseController
{
    public QaaController(IMediator mediator, ILogger<QaaController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("download-summary")]
    [ProducesResponseType(typeof(GetQaaDownloadSummaryQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDownloadSummary()
    {
        return await SendRequestAsync(new GetQaaDownloadSummaryQuery());
    }

    [HttpGet("download")]
    [ProducesResponseType(typeof(GetQaaQualificationsExportQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download([FromQuery] string username)
    {
        return await SendRequestAsync(new GetQaaQualificationsExportQuery
        {
            CurrentUsername = username
        });
    }
}
