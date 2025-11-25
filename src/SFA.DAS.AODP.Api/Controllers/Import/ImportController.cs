using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Api.Requests;
using SFA.DAS.AODP.Application.Commands.Import;

namespace SFA.DAS.AODP.Api.Controllers.Import;

[ApiController]
[Route("api/[controller]")]
public class ImportController : BaseController
{

    public ImportController(IMediator mediator, ILogger<ImportController> logger) : base(mediator, logger)
    {
    }

    [HttpPost("defunding-list")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportDefundingList([FromForm] ImportDefundingListRequest request)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        var command = new ImportDefundingListCommand
        {
            File = file,
            FileName = file.FileName
        };

        return await SendRequestAsync(command);
    }

    [HttpPost("pldns")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportPLDNS([FromForm] ImportPLDNSRequest request)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        var command = new ImportPLDNSCommand
        {
            File = file,
            FileName = file.FileName
        };

        return await SendRequestAsync(command);
    }
}
