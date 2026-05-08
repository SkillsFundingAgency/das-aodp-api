using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Api;
using SFA.DAS.AODP.Application.Commands.Files;
using SFA.DAS.AODP.Application.Queries.Files;

namespace SFA.DAS.AODP.Api.Controllers.Files;

[ApiController]
[Route("api/files")]
public class FilesController : BaseController
{

    public FilesController(IMediator mediator, ILogger<FilesController> logger) : base(mediator, logger)
    {
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateFile(
    [FromBody] CreateFileMetadataCommand command)
    {
        return await SendRequestAsync(command);
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        return await SendRequestAsync(new DeleteFileCommand { FileId = fileId });
    }


    [HttpPost]
    public async Task<IActionResult> GetFiles([FromBody] GetFileMetadataQuery fileMetadataQuery)
    {
        return await SendRequestAsync(fileMetadataQuery);
    }
}