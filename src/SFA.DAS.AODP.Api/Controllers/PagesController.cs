﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Api.Controllers
{
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
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(Guid sectionId)
        {
            var query = new GetAllPagesQueryRequest(new())
            {
                SectionId = sectionId
            };

            var response = await _mediator.Send(query);
            if (response.Success)
            {
                return Ok(response);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpGet("/api/pages/{pageId}/section/{sectionId}")]
        [ProducesResponseType(typeof(GetPageByIdQueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid pageId, Guid sectionId)
        {
            var query = new GetPageByIdQuery(pageId, sectionId);

            var response = await _mediator.Send(query);

            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpPost("/api/pages")]
        [ProducesResponseType(typeof(CreatePageCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePageCommand.Page page)
        {
            var command = new CreatePageCommand(page);

            var response = await _mediator.Send(command);
            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpPut("/api/pages/{pageId}")]
        [ProducesResponseType(typeof(UpdatePageCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid pageId, [FromBody] UpdatePageCommand.Page page)
        {
            var command = new UpdatePageCommand(pageId, page);

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpDelete("/api/pages/{pageId}")]
        [ProducesResponseType(typeof(DeletePageCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid pageId)
        {
            var command = new DeletePageCommand(pageId);

            var response = await _mediator.Send(command);
            if (response.Success)
            {
                return Ok(response);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }
    }
}