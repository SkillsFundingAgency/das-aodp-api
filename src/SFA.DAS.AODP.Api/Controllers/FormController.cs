using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Form;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Models.Forms.FormBuilder;
using SFA.DAS.AODP.Models.ApiResponses.Form;

namespace SFA.DAS.AODP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : Controller
    {
        private readonly IMediator _mediator;

        public FormController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("/api/forms")]
        public async Task<IActionResult> GetFormsAsync()
        {
            var response = await _mediator.Send(new GetAllFormsQuery());
            if (response.Success)
            {
                return Ok(new GetAllFormsResponse() { FormVersions = response.Data });
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [AllowAnonymous]
        [HttpGet("/api/forms/{formVersionId}")]
        public async Task<IActionResult> GetFormsAsync(Guid formVersionId)
        {
            var response = await _mediator.Send(new GetFormByVersionIdQuery() { Id = formVersionId });
            if (response.Success)
            {
                if (response.Data is null)
                    return NotFound();
                return Ok(new GetFormVersionByVersionIdResponse() { FormVersion = response.Data });
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }

        [AllowAnonymous]
        [HttpPost("/api/forms")]
        public async Task<IActionResult> Create([FromBody] FormVersion formVersion)
        {
            var command = new CreateFormCommand
            {
                FormVersion = formVersion,
            };

            var createFormResponse = await _mediator.Send(command);
            return Ok(createFormResponse);
        }

        [AllowAnonymous]
        [HttpPut("/api/forms/{formVersionId}")]
        public async Task<IActionResult> Update(Guid formVersionId, [FromBody] FormVersion formVersion)
        {
            var command = new UpdateFormCommand
            {
                FormVersion = formVersion,
            };

            var updatedFormResponse = await _mediator.Send(command);
            return Ok(updatedFormResponse);
        }

        [AllowAnonymous]
        [HttpPut("/api/forms/{formVersionId}/publish")]
        public async Task<IActionResult> Publish(Guid formVersionId)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("/api/forms/{formVersionId}/unpublish")]
        public async Task<IActionResult> UnPublish(Guid formVersionId)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("/api/forms/{formVersionId}")]
        public async Task<IActionResult> RemoveForm(Guid formVersionId)
        {
            var command = new DeleteFormCommand
            {
                Id = formVersionId,
            };

            var deleteStatus = await _mediator.Send(command);
            return Ok(deleteStatus);
        }
    }
}
