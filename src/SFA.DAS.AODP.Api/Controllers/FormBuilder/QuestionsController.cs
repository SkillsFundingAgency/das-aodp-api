using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;

public class QuestionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IMediator mediator, ILogger<QuestionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions")]
    [ProducesResponseType(typeof(CreateQuestionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromBody] CreateQuestionCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;
        command.PageId = pageId;

        var response = await _mediator.Send(command);
        if (response.Success && response.Id != default)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to add question with page Id `{command.PageId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is DependantNotFoundException)
        {
            _logger.LogError(message: $"Request to add question with page Id `{command.PageId}` but no page with this Id can be found. ", exception: response.InnerException);
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(UpdateQuestionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId, [FromBody] UpdateQuestionCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;
        command.Id = questionId;
        command.PageId = pageId;

        var response = await _mediator.Send(command);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is LockedRecordException)
        {
            _logger.LogError($"Request to update question with question Id `{questionId}` and page Id `{pageId}` but form version is locked. ");
            return Forbid();
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to edit question with question Id `{questionId}` but no question with this Id can be found. ");
            return NotFound();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/MoveDown")]
    [ProducesResponseType(typeof(UpdateQuestionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveDownAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId, [FromBody] UpdateQuestionCommand command)
    {
        var query = new MoveQuestionDownCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId,
            QuestionId = questionId,
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move page down with question Id `{questionId}` and form page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move down with question Id `{questionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/MoveUp")]
    [ProducesResponseType(typeof(UpdateQuestionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveUpAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId, [FromBody] UpdateQuestionCommand command)
    {
        var query = new MoveQuestionUpCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId,
            QuestionId = questionId,
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request to move page up with question Id `{questionId}` and form page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting section to move up with question Id `{questionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(GetQuestionByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid formVersionId, Guid pageId, Guid sectionId, Guid questionId)
    {
        var query = new GetQuestionByIdQuery()
        {
            QuestionId = questionId,
            FormVersionId = formVersionId,
            SectionId = sectionId,
            PageId = pageId
        };

        var response = await _mediator.Send(query);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.InnerException is NotFoundException)
        {
            _logger.LogError($"Request for question with question Id `{questionId}` and page Id `{pageId}` returned 404 (not found).");
            return NotFound();
        }

        _logger.LogError(message: $"Error thrown getting question for Id `{questionId}` and page Id `{pageId}`.", exception: response.InnerException);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
