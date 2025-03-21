using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommandHandler : IRequestHandler<SaveQualificationsFundingOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _repository;

        public SaveQualificationsFundingOffersDetailsCommandHandler(IQualificationFundingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersDetailsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _repository.GetByIdAsync(request.QualificationVersionId);

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