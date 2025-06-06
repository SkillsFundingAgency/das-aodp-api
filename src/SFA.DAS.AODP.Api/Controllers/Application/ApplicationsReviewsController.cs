﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Application.Queries.Application.Review;

namespace SFA.DAS.AODP.Api.Controllers.Application;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsReviewsController : BaseController
{
    public ApplicationsReviewsController(IMediator mediator, ILogger<ApplicationsReviewsController> logger) : base(mediator, logger)
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

    [HttpGet("/api/application-reviews/{applicationReviewId}")]
    [ProducesResponseType(typeof(GetApplicationForReviewByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplicationForReviewById(Guid applicationReviewId)
    {
        return await SendRequestAsync(new GetApplicationForReviewByIdQuery(applicationReviewId));
    }


    [HttpGet("/api/application-reviews/{applicationReviewId}/share-status")]
    [ProducesResponseType(typeof(GetApplicationReviewSharingStatusByIdQueryHandler), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplicationReviewSharingStatusById(Guid applicationReviewId)
    {
        return await SendRequestAsync(new GetApplicationReviewSharingStatusByIdQuery(applicationReviewId));
    }

    [HttpGet("/api/application-reviews/{applicationReviewId}/feedback/{userType}")]
    [ProducesResponseType(typeof(GetFeedbackForApplicationReviewByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFeedbackForApplicationReviewById(Guid applicationReviewId, string userType)
    {
        return await SendRequestAsync(new GetFeedbackForApplicationReviewByIdQuery(applicationReviewId, userType));
    }


    [HttpPut("/api/application-reviews/{applicationReviewId}/qfau-outcome")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveFundingOfferOutcome(SaveQfauFundingReviewOutcomeCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/qfau-decision")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveQfauFundingReviewDecision(SaveQfauFundingReviewDecisionCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }


    [HttpPut("/api/application-reviews/{applicationReviewId}/qfau-offers")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveFundingOffers(SaveQfauFundingReviewOffersCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/qfau-offer-details")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveFundingOfferDetails(SaveQfauFundingReviewOffersDetailsCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;

        return await SendRequestAsync(command);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/ofqual-outcome")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveOfqualOutcome(SaveOfqualReviewOutcomeCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/skills-england-outcome")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveSkillsEnglandReviewOutcome(SaveSkillsEnglandReviewOutcomeCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }


    [HttpPut("/api/application-reviews/{applicationReviewId}/owner")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveOwner(SaveReviewOwnerUpdateCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }

    [HttpPut("/api/application-reviews/{applicationReviewId}/qan")]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveQan(SaveQanCommand command, Guid applicationReviewId)
    {
        command.ApplicationReviewId = applicationReviewId;
        return await SendRequestAsync(command);
    }

    [HttpGet("/api/application-reviews/{applicationReviewId}/form-answers")]
    [ProducesResponseType(typeof(GetApplicationFormAnswersByReviewIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplicationAnswersByIdAsync(Guid applicationReviewId)
    {
        var query = new GetApplicationFormAnswersByReviewIdQuery(applicationReviewId);
        return await SendRequestAsync(query);
    }
}
