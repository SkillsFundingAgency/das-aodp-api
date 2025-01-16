using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Form;
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
                return Ok(new GetAllFormsResponse() { Forms = response.Data });
            }

            var errorObjectResult = new ObjectResult(response.ErrorMessage);
            errorObjectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return errorObjectResult;
        }
    }
}
