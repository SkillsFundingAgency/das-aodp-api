using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IMediator mediator, ILogger<ApplicationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/applications/forms/{formVersionId}/sections/{sectionId}/pages/{pageOrder}")]
    [ProducesResponseType(typeof(GetApplicationPageByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(int pageOrder, Guid sectionId, Guid formVersionId)
    {
        var query = new GetApplicationPageByIdQuery(pageOrder, sectionId, formVersionId);

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request for page with section Id `{sectionId}` and page order `{pageOrder}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting page for section Id `{sectionId}` and page order `{pageOrder}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/applications/{applicationId}/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}")]
    [ProducesResponseType(typeof(UpdateQuestionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAnswersAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid applicationId, [FromBody] UpdatePageAnswersCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;
        command.ApplicationId = applicationId;
        command.PageId = pageId;

        var response = await _mediator.Send(command);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to update page with page Id `{pageId}` but application is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to update page with page Id `{pageId}` but page could not be found. ");
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("/api/applications")]
    [ProducesResponseType(typeof(CreateApplicationCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateApplicationCommand command)
    {
        var response = await _mediator.Send(command);
        if (response.Success)
        {
            return Ok(response);
        }

        _logger.LogError(message: $"Error thrown creating a application.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

}
