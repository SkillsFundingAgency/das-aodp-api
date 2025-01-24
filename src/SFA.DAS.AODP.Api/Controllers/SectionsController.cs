using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

namespace SFA.DAS.AODP.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/api/sections/form/{formVersionId}")]
    [ProducesResponseType(typeof(GetAllSectionsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid formVersionId)
    {
        var query = new GetAllSectionsQuery(formVersionId);

        var response = await _mediator.Send(query);
        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("/api/sections/{sectionId}/form/{formVersionId}")]
    [ProducesResponseType(typeof(GetSectionByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid sectionId, [FromRoute] Guid formVersionId)
    {
        var query = new GetSectionByIdQuery(sectionId, formVersionId);

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

    [HttpPost("/api/sections")]
    [ProducesResponseType(typeof(CreateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSectionCommand.Section section)
    {
        var command = new CreateSectionCommand(section);

        var response = await _mediator.Send(command);
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

    [HttpPut("/api/sections/{formVersionId}")]
    [ProducesResponseType(typeof(UpdateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid formVersionId, [FromBody] UpdateSectionCommand.Section section)
    {
        var command = new UpdateSectionCommand(formVersionId, section);

        var response = await _mediator.Send(command);

        if (response.Success && response.Data is not null)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("/api/sections/{sectionId}")]
    [ProducesResponseType(typeof(DeleteSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAsync([FromRoute] Guid sectionId)
    {
        var command = new DeleteSectionCommand(sectionId);

        var response = await _mediator.Send(command);
        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
