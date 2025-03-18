using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOffersCommandHandler : IRequestHandler<SaveQfauFundingReviewOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFundingRepository _fundingRepository;
        private readonly IApplicationReviewFeedbackRepository _feedbackRepository;

        public SaveQfauFundingReviewOffersCommandHandler(IApplicationReviewFundingRepository repository, IApplicationReviewFeedbackRepository feedbackRepository)
        {
            _fundingRepository = repository;
            _feedbackRepository = feedbackRepository;
        }


        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewOffersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _fundingRepository.GetByReviewIdAsync(request.ApplicationReviewId);

                List<ApplicationReviewFunding> create = new();
                List<ApplicationReviewFunding> remove = new();

                foreach (var offerId in request.SelectedOfferIds)
                {
                    var offer = fundedOffers.FirstOrDefault(a => a.FundingOfferId == offerId);
                    if (offer == null) create.Add(new()
                    {
                        ApplicationReviewId = request.ApplicationReviewId,
                        FundingOfferId = offerId
                    });
                }

                foreach (var offer in fundedOffers)
                {
                    if (!request.SelectedOfferIds.Contains(offer.FundingOfferId))
                    {
                        remove.Add(offer);
                    }
                }

                if (remove.Count > 0) await _fundingRepository.RemoveAsync(remove);
                if (create.Count > 0) await _fundingRepository.CreateAsync(create);

                var review = await _feedbackRepository.GeyByReviewIdAndUserType(request.ApplicationReviewId, Models.Application.UserType.Qfau);
                if (review != null)
                {
                    review.LatestCommunicatedToAwardingOrganisation = false;
                    await _feedbackRepository.UpdateAsync(review);
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
                response.Success = false;
            }
            return response;
        }
    }

}