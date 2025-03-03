using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Application.Queries.Application.Review;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsReviewController : BaseController
{
    public ApplicationsReviewController(IMediator mediator, ILogger<ApplicationsReviewController> logger) : base(mediator, logger)
    { }

    [HttpPost("/api/application-reviews")]
    [ProducesResponseType(typeof(GetApplicationFormsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplicationReviews(GetApplicationsForReviewQuery query)
    {
        return await SendRequestAsync(query);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/share")]
    [ProducesResponseType(typeof(GetApplicationFormsQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateApplicationReviewSharing(Guid applicationReviewId, UpdateApplicationReviewSharingCommand command)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }
}
