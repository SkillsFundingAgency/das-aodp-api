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

    public PagesController(IMediator mediator)
    {
        _mediator = mediator;
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
            return NotFound();
        }

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
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
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
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
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
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}