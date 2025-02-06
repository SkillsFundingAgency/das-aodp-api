using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;



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

    [HttpGet("/api/forms/{formVersionId}/sections")]
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

    [HttpGet("/api/forms/{formVersionId}/sections/{sectionId}")]
    [ProducesResponseType(typeof(GetSectionByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid sectionId, [FromRoute] Guid formVersionId)
    {
        var query = new GetSectionByIdQuery(sectionId, formVersionId);

        var response = await _mediator.Send(query);

        if (response.Success)
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

    [HttpPost("/api/forms/{formVersionId}/sections")]
    [ProducesResponseType(typeof(CreateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(Guid formVersionId, [FromBody] CreateSectionCommand command)
    {
        command.FormVersionId = formVersionId;

        var response = await _mediator.Send(command);
        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError($"Request to add section to form version Id `{command.FormVersionId}` but no form version with this Id can be found. ");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown creating new section on form version Id `{command.FormVersionId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}")]
    [ProducesResponseType(typeof(UpdateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromBody] UpdateSectionCommand command)
    {
        command.FormVersionId = formVersionId;
        command.Id = sectionId;
        var response = await _mediator.Send(command);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to update section with section Id `{sectionId}` and form version Id `{formVersionId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to update section with section Id `{sectionId}` and form version Id `{formVersionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown updating section with form version Id `{formVersionId}` and section Id `{sectionId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/MoveUp")]
    [ProducesResponseType(typeof(UpdateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveUpAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId)
    {
        var query = new MoveSectionUpCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move section up with section Id `{sectionId}` and form version Id `{formVersionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move up with form version Id `{formVersionId}` and section Id `{sectionId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/MoveDown")]
    [ProducesResponseType(typeof(UpdateSectionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveDownAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId)
    {
        var query = new MoveSectionDownCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move section down with section Id `{sectionId}` and form version Id `{formVersionId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move down with form version Id `{formVersionId}` and section Id `{sectionId}`.", exception: response.InnerException);
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
