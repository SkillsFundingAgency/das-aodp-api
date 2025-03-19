using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOffersCommandHandler : IRequestHandler<SaveQfauFundingReviewOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFundingRepository _repository;

        public SaveQfauFundingReviewOffersCommandHandler(IApplicationReviewFundingRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewOffersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _repository.GetByReviewIdAsync(request.ApplicationReviewId);

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

                if (remove.Count > 0) await _repository.RemoveAsync(remove);
                if (create.Count > 0) await _repository.CreateAsync(create);

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