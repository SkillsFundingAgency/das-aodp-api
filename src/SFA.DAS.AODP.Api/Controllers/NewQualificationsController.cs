using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Queries.Qualifications;

namespace SFA.DAS.AODP.Api.Controllers
{
    [ApiController]
    [Route("api/new-qualifications")]
    public class NewQualificationsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAllNewQualifications()
        {
            var result = await _mediator.Send(new GetNewQualificationsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQualificationDetails(int id)
        {
            var result = await _mediator.Send(new GetQualificationDetailsQuery { Id = id });

            if (!result.Success)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
