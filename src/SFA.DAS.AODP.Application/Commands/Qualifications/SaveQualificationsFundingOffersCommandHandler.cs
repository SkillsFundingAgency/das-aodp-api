using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersCommandHandler : IRequestHandler<SaveQualificationsFundingOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _repository;

        public SaveQualificationsFundingOffersCommandHandler(IQualificationFundingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var qualificationfundedOffers = await _repository.GetByIdAsync(request.QualificationVersionId);

                List<QualificationFundings> create = new();
                List<QualificationFundings> remove = new();

                foreach (var offerId in request.SelectedOfferIds)
                {
                    var offer = qualificationfundedOffers.FirstOrDefault(a => a.FundingOfferId == offerId);
                    if (offer == null) create.Add(new()
                    {
                        QualificationVersionId = request.QualificationVersionId,
                        FundingOfferId = offerId
                    });
                }

                foreach (var offer in qualificationfundedOffers)
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