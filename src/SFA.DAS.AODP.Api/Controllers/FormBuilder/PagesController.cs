using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;
[ApiController]
[Route("api/[controller]")]
public class PagesController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<FormsController> _logger;

    public PagesController(IMediator mediator, ILogger<FormsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/forms/{formVersionId}/sections/{sectionId}/pages")]
    [ProducesResponseType(typeof(GetAllPagesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(Guid sectionId)
    {
        var query = new GetAllPagesQuery(sectionId);

        var response = await _mediator.Send(query);
        if (response.Success)
        {
            return Ok(response);
        }

        _logger.LogError(message: $"Error thrown getting all pages for section Id `{sectionId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}")]
    [ProducesResponseType(typeof(GetPageByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid formVersionId, Guid pageId, Guid sectionId)
    {
        var query = new GetPageByIdQuery(pageId, sectionId, formVersionId);

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request for page with section Id `{sectionId}` and page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting page for section Id `{sectionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/preview")]
    [ProducesResponseType(typeof(GetPageByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPagePreviewByIdAsync(Guid formVersionId, Guid pageId, Guid sectionId)
    {
        var query = new GetPagePreviewByIdQuery(pageId, sectionId, formVersionId);

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request for page preview with section Id `{sectionId}` and page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting page preview for section Id `{sectionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("/api/forms/{formVersionId}/sections/{sectionId}/pages")]
    [ProducesResponseType(typeof(CreatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromBody] CreatePageCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;

        var response = await _mediator.Send(command);
        if (response.Success && response.Id != default)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to add page with section Id `{command.SectionId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError(message: $"Request to add page with section Id `{command.SectionId}` but no section with this Id can be found. ", exception: response.InnerException);
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}")]
    [ProducesResponseType(typeof(UpdatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromBody] UpdatePageCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;
        command.Id = pageId;

        var response = await _mediator.Send(command);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to update page with section Id `{sectionId}` and page Id `{pageId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to edit page with page Id `{pageId}` but no page with this Id can be found. ");
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/MoveUp")]
    [ProducesResponseType(typeof(UpdatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveUpAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId)
    {
        var query = new MovePageUpCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move page up with section Id `{sectionId}` and form page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move up with section Id `{sectionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }


    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/MoveDown")]
    [ProducesResponseType(typeof(UpdatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveDownAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId)
    {
        var query = new MovePageDownCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move page down with section Id `{sectionId}` and form page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move down with section Id `{sectionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}")]
    [ProducesResponseType(typeof(DeletePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAsync([FromRoute] Guid pageId)
    {
        var command = new DeletePageCommand(pageId);

        var response = await _mediator.Send(command);
        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to delete page with page Id `{pageId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to delete page with page Id `{pageId}` but no page with this Id can be found. ");
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}