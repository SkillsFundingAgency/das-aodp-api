﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Routes;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SFA.DAS.AODP.Api.Controllers.FormBuilder;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<RoutesController> _logger;

    public RoutesController(IMediator mediator, ILogger<RoutesController> logger) : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/routes/forms/{formVersionId}/available-sections")]
    [ProducesResponseType(typeof(GetAvailableSectionsAndPagesForRoutingQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableSectionsAndPagesForRouting(Guid formVersionId)
    {
        var query = new GetAvailableSectionsAndPagesForRoutingQuery()
        {
            FormVersionId = formVersionId
        };

        return await SendRequestAsync(query);
    }

    [HttpGet("/api/routes/forms/{formVersionId}")]
    [ProducesResponseType(typeof(GetRoutingInformationForFormQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoutesByFormVersionId(Guid formVersionId)
    {
        var query = new GetRoutingInformationForFormQuery()
        {
            FormVersionId = formVersionId
        };

        return await SendRequestAsync(query);
    }

    [HttpGet("/api/routes/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/available-questions")]
    [ProducesResponseType(typeof(GetAvailableQuestionsForRoutingQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableQuestionsForRouting(Guid pageId)
    {
        var query = new GetAvailableQuestionsForRoutingQuery()
        {
            PageId = pageId
        };

        return await SendRequestAsync(query);
    }

    [HttpGet("/api/routes/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(GetRoutingInformationForQuestionQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestionRoutingInformation(Guid questionId, Guid formVersionId)
    {
        var query = new GetRoutingInformationForQuestionQuery()
        {
            QuestionId = questionId,
            FormVersionId = formVersionId
        };

        return await SendRequestAsync(query);
    }

    [HttpPut("/api/routes/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(GetRoutingInformationForQuestionQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfigureRoutingForQuestion(ConfigureRoutingForQuestionCommand command)
    {
        return await SendRequestAsync(command);
    }

    [HttpDelete("/api/routes/forms/{formVersionId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRouteAsync(Guid formVersionid, Guid sectionId, Guid pageId, Guid questionId)
    {
        return await SendRequestAsync(new DeleteRouteCommand()
        {
            FormVersionId = formVersionid,
            SectionId = sectionId,
            PageId = pageId,
            QuestionId = questionId
        });
    }
}
