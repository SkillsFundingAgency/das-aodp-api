using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

namespace SFA.DAS.AODP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : Controller
{
    private readonly IMediator _mediator;

    public FormsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/api/forms")]
    [ProducesResponseType(typeof(GetAllFormVersionsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new GetAllFormVersionsQuery();
        var response = await _mediator.Send(query);
        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("/api/forms/{formVersionId}")]
    [ProducesResponseType(typeof(GetFormVersionByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid formVersionId)
    {
        var query = new GetFormVersionByIdQuery(formVersionId);

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

    [HttpPost("/api/forms")]
    [ProducesResponseType(typeof(CreateFormVersionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFormVersionCommand.FormVersion formVersion)
    {
        var command = new CreateFormVersionCommand(formVersion);

        var response = await _mediator.Send(command);
        if (response.Success && response.Data is not null)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}")]
    [ProducesResponseType(typeof(UpdateFormVersionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(Guid formVersionId, [FromBody] UpdateFormVersionCommand.FormVersion formVersion)
    {
        var command = new UpdateFormVersionCommand(formVersionId, formVersion);

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

    [HttpPut("/api/forms/{formVersionId}/publish")]
    [ProducesResponseType(typeof(UpdateFormVersionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PublishAsync(Guid formVersionId)
    {
        var command = new PublishFormVersionCommand(formVersionId);
        
        var response = await _mediator.Send(command);

        if (response.Success)
            return Ok(response);

        if (response.InnerException is NotFoundException)
        {
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/unpublish")]
    [ProducesResponseType(typeof(UpdateFormVersionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnpublishAsync(Guid formVersionId)
    {
        var command = new UnpublishFormVersionCommand(formVersionId);

        var response = await _mediator.Send(command);

        if (response.Success)
            return Ok(response);

        if (response.InnerException is NotFoundException)
        {
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("/api/forms/{formVersionId}")]
    [ProducesResponseType(typeof(DeleteFormVersionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAsync(Guid formVersionId)
    {
        var command = new DeleteFormVersionCommand(formVersionId);

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
