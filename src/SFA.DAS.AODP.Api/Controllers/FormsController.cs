using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Api.Controllers
{
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
        [ProducesResponseType(typeof(List<FormVersion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var query = new GetAllFormVersionsQueryRequest();
            var response = await _mediator.Send(query);
            if (response.Success)
            {
                return Ok(response.Data);
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [HttpGet("/api/forms/{formVersionId}")]
        [ProducesResponseType(typeof(FormVersion), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid formVersionId)
        {
            var query = new GetFormVersionByIdQueryRequest(formVersionId);

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

        [HttpPost("/api/forms")]
        [ProducesResponseType(typeof(FormVersion), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] FormVersion formVersion)
        {
            var command = new CreateFormVersionCommandRequest
            {
                Data = formVersion,
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

        [HttpPut("/api/forms/{formVersionId}")]
        [ProducesResponseType(typeof(FormVersion), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid formVersionId, [FromBody] FormVersion formVersion)
        {
            var command = new UpdateFormVersionCommandRequest
            {
                Data = formVersion,
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

        [HttpPut("/api/forms/{formVersionId}/publish")]
        public async Task<IActionResult> PublishAsync(Guid formVersionId)
        {
            return Ok();
        }

        [HttpPut("/api/forms/{formVersionId}/unpublish")]
        public async Task<IActionResult> UnpublishAsync(Guid formVersionId)
        {
            return Ok();
        }

        [HttpDelete("/api/forms/{formVersionId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveAsync(Guid formVersionId)
        {
            var command = new DeleteFormVersionCommandRequest
            {
                Id = formVersionId,
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
