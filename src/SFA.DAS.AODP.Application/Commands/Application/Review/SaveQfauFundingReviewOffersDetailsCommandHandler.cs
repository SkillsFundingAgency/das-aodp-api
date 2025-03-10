using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOffersDetailsCommandHandler : IRequestHandler<SaveQfauFundingReviewOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFundingRepository _repository;

        public SaveQfauFundingReviewOffersDetailsCommandHandler(IApplicationReviewFundingRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewOffersDetailsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _repository.GetByReviewIdAsync(request.ApplicationReviewId);

                foreach (var detail in request.Details)
                {
                    var offer = fundedOffers.FirstOrDefault(a => a.FundingOfferId == detail.FundingOfferId) ?? throw new RecordNotFoundException(detail.FundingOfferId);

                    offer.StartDate = detail.StartDate;
                    offer.EndDate = detail.EndDate;
                    offer.Comments = detail.Comments;
                }

                await _repository.UpdateAsync(fundedOffers);

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