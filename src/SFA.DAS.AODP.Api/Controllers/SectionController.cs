using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AODP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("/api/sections/{formVersionId}")]
        public async Task<IActionResult> GetSectionsForForm(Guid formVersionId)
        {
            return Ok();
        }

        // This endpoint will create a new section generating a key and ID 
        [AllowAnonymous]
        [HttpPost("/api/sections/{formVersionId}")]
        public async Task<IActionResult> CreateSection(Guid formVersionId)
        {
            return Ok();
        }

        // This endpoint will create a new section generating a key and ID 
        [AllowAnonymous]
        [HttpPut("/api/sections/{formVersionId}/section/{sectionKey}")]
        public async Task<IActionResult> UpdateSection(Guid formVersionId, Guid sectionKey)
        {
            return Ok();
        }
    }
}
