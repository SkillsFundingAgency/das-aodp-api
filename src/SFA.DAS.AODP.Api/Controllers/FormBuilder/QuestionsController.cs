using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IMediator mediator, ILogger<QuestionsController> logger) : base(mediator, logger)
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

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId, [FromBody] UpdateQuestionCommand command)
    {
        command.FormVersionId = formVersionId;
        command.SectionId = sectionId;
        command.Id = questionId;
        command.PageId = pageId;

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/MoveDown")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveDownAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId)
    {
        var query = new MoveQuestionDownCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId,
            QuestionId = questionId,
        };

        return await SendRequestAsync(query);
    }

    [HttpPut("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/MoveUp")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveUpAsync([FromRoute] Guid formVersionId, [FromRoute] Guid sectionId, [FromRoute] Guid pageId, [FromRoute] Guid questionId)
    {
        var query = new MoveQuestionUpCommand()
        {
            SectionId = sectionId,
            FormVersionId = formVersionId,
            PageId = pageId,
            QuestionId = questionId,
        };

        return await SendRequestAsync(query);
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

        return await SendRequestAsync(query);
    }

    [HttpDelete("/api/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(EmptyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid questionId)
    {
        var query = new DeleteQuestionCommand()
        {
            QuestionId = questionId
        };

        return await SendRequestAsync(query);
    }
}
