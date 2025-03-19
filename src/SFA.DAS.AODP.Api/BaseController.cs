using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Api;

public class BaseController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public BaseController(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> SendRequestAsync<TResponse>(IRequest<BaseMediatrResponse<TResponse>> request) where TResponse : class, new()
    {
        var response = await _mediator.Send(request);

        if (response.Success)
        {
            return Ok(response.Value);
        }


        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError(response.InnerException, $"The record is locked.");
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError(response.InnerException, $"The dependent record was not found.");
            return NotFound();
        }

        if (response.InnerException is NotFoundException or NotFoundWithNameException)
        {
            _logger.LogWarning($"The record was not found.");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown handling request.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
