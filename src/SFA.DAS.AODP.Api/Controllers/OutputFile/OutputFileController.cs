using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AODP.Api.Controllers.OutputFile;

[ApiController]
[Route("api/[controller]")]
public class OutputFileController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<OutputFileController> _logger;

    public OutputFileController(IMediator mediator, ILogger<OutputFileController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/outputfiles")]
    [ProducesResponseType(typeof(GetPreviousOutputFilesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        var rng = new Random();
        return Ok(new GetPreviousOutputFilesQueryResponse
        {
            GeneratedFiles = new List<GetPreviousOutputFilesQueryResponse.File>
            {
                new()
                {
                    DisplayName = "Hello World",
                    IsInProgress = true,
                    Created = DateTime.UtcNow
                },
                new()
                {
                    DisplayName = "Hello World 2",
                    BlobName = "helloworld2.txt",
                    IsInProgress = false,
                    Created = DateTime.UtcNow
                        .AddDays(0 - rng.Next(1, 4))
                        .AddHours(0 - rng.Next(3, 7))
                        .AddMinutes(0 - rng.Next(2, 50))
                },
                new()
                {
                    DisplayName = "Hello World 3",
                    BlobName = "helloworld3.txt",
                    IsInProgress = false,
                    Created = DateTime.UtcNow
                        .AddDays(0 - rng.Next(4, 6))
                        .AddHours(0 - rng.Next(3, 7))
                        .AddMinutes(0 - rng.Next(2, 50))
                },
                new()
                {
                    DisplayName = "Hello World 4",
                    BlobName = "helloworld4.txt",
                    IsInProgress = false,
                    Created = DateTime.UtcNow
                        .AddDays(0 - rng.Next(6, 10))
                        .AddHours(0 - rng.Next(3, 7))
                        .AddMinutes(0 - rng.Next(2, 50))
                },
                new()
                {
                    DisplayName = "Hello World 5",
                    BlobName = "helloworld5.txt",
                    IsInProgress = false,
                    Created = DateTime.UtcNow
                        .AddDays(0 - rng.Next(10, 14))
                        .AddHours(0 - rng.Next(3, 7))
                        .AddMinutes(0 - rng.Next(2, 50))
                },
            }
        });
    }
}

public class GetPreviousOutputFilesQueryResponse
{
    public List<File> GeneratedFiles { get; set; } = new List<File>();
    public class File
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? BlobName { get; set; }
        public bool IsInProgress { get; set; }
        public DateTime? Created { get; set; }
    }
}
