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
    private readonly ILogger<SectionsController> _logger;

    public SectionsController(IMediator mediator, ILogger<SectionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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

        _logger.LogError(message: $"Error thrown getting all sections for form version Id `{formVersionId}`.", exception: response.InnerException);
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
            _logger.LogError($"Request for section with section Id `{sectionId}` and form version Id `{formVersionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section with form version Id `{formVersionId}` and section Id `{sectionId}`.", exception: response.InnerException);
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

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError($"Request to add section to form version Id `{section.FormVersionId}` but no form version with this Id can be found. ");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown creating new section on form version Id `{section.FormVersionId}`.", exception: response.InnerException);
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
            _logger.LogError($"Request to update section with section Id `{section.Id}` and form version Id `{formVersionId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to update section with section Id `{section.Id}` and form version Id `{formVersionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown updating section with form version Id `{formVersionId}` and section Id `{section.Id}`.", exception: response.InnerException);
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
            _logger.LogError($"Request to delete section with section Id `{sectionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown deleting section with the section Id `{sectionId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
