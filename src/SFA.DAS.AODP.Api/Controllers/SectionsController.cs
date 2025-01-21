using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/sections/form/{formId}")]
        [ProducesResponseType(typeof(List<Section>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid formId)
        {
            var query = new GetAllSectionsQueryRequest(new())
            {
                FormId = formId
            };
            var response = await _mediator.Send(query);
            if (response.Success)
            {
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpGet("/api/sections/{sectionId}/form/{formId}")]
        [ProducesResponseType(typeof(List<Page>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid sectionId, [FromRoute] Guid formId)
        {
            var query = new GetSectionByIdQueryRequest()
            {
                Id = sectionId
            };

            var response = await _mediator.Send(query);

            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpPost("/api/sections")]
        [ProducesResponseType(typeof(Section), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] Section model)
        {
            var command = new CreateSectionCommandRequest
            {
                Data = model,
            };

            var response = await _mediator.Send(command);
            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpPut("/api/sections/{formId}")]
        [ProducesResponseType(typeof(Section), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid formId, [FromBody] Section model)
        {
            var command = new UpdateSectionCommandRequest
            {
                Data = model,
            };

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpDelete("/api/sections/{sectionId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid sectionId)
        {
            var command = new DeleteSectionCommandRequest
            {
                Id = sectionId,
            };

            var response = await _mediator.Send(command);
            if (response.Success)
            {
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }
    }
}
