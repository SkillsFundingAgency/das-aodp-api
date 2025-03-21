﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Queries.Application.Message;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class ApplicationMessagesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationMessagesController> _logger;

    public ApplicationMessagesController(IMediator mediator, ILogger<ApplicationMessagesController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/applications/{applicationId}/messages")]
    [ProducesResponseType(typeof(GetApplicationMessagesByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplicationMessagesByIdAsync([FromRoute] Guid applicationId, [FromQuery] string userType)
    {
        var query = new GetApplicationMessagesByIdQuery(applicationId, userType);
        return await SendRequestAsync(query);
    }

    [HttpPost("/api/applications/{applicationId}/messages")]
    [ProducesResponseType(typeof(CreateApplicationMessageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateApplicationMessageAsync([FromBody] CreateApplicationMessageCommand command, [FromRoute] Guid applicationId)
    {
        command.ApplicationId = applicationId;

        var response = await _mediator.Send(command);
        if (response.Success)
        {
            return Ok(response.Value);
        }

        _logger.LogError(message: $"Error thrown updating a application.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
