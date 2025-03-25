using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOffersDetailsCommandHandler : IRequestHandler<SaveQfauFundingReviewOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFundingRepository _fundingRepository;
        private readonly IApplicationReviewFeedbackRepository _feedbackRepository;

        public SaveQfauFundingReviewOffersDetailsCommandHandler(IApplicationReviewFundingRepository repository, IApplicationReviewFeedbackRepository feedbackRepository)
        {
            _fundingRepository = repository;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewOffersDetailsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _fundingRepository.GetByReviewIdAsync(request.ApplicationReviewId);

                foreach (var detail in request.Details)
                {
                    var offer = fundedOffers.FirstOrDefault(a => a.FundingOfferId == detail.FundingOfferId) ?? throw new RecordNotFoundException(detail.FundingOfferId);
                    offer.StartDate = detail.StartDate;
                    offer.EndDate = detail.EndDate;
                    offer.Comments = detail.Comments;
                }

                await _fundingRepository.UpdateAsync(fundedOffers);

                var review = await _feedbackRepository.GeyByReviewIdAndUserType(request.ApplicationReviewId, Models.Application.UserType.Qfau);
                if(review != null)
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