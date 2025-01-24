using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Api.Controllers;

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

    [HttpGet("/api/pages/section/{sectionId}")]
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

    [HttpGet("/api/pages/{pageId}/section/{sectionId}")]
    [ProducesResponseType(typeof(GetPageByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid pageId, Guid sectionId)
    {
        var query = new GetPageByIdQuery(pageId, sectionId);

        var response = await _mediator.Send(query);

        if (response.Success && response.Data is not null)
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

    [HttpPost("/api/pages")]
    [ProducesResponseType(typeof(CreatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePageCommand.Page page)
    {
        var command = new CreatePageCommand(page);

        var response = await _mediator.Send(command);
        if (response.Success && response.Data is not null)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to add page with section Id `{page.SectionId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError(message: $"Request to add page with section Id `{page.SectionId}` but no section with this Id can be found. ", exception: response.InnerException);
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/pages/{pageId}")]
    [ProducesResponseType(typeof(UpdatePageCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid pageId, [FromBody] UpdatePageCommand.Page page)
    {
        var command = new UpdatePageCommand(pageId, page);

        var response = await _mediator.Send(command);

        if (response.Success && response.Data is not null)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to update page with section Id `{page.SectionId}` and page Id `{pageId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to edit page with page Id `{pageId}` but no page with this Id can be found. ");
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("/api/pages/{pageId}")]
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