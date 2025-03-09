using MediatR;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;

namespace SFA.DAS.AODP.Application.Queries.Offers
{
    public class GetFundingOffersQueryHandler : IRequestHandler<GetFundingOffersQuery, BaseMediatrResponse<GetFundingOffersQueryResponse>>
    {
        private readonly IFundingOfferRepository _repository;

        public GetFundingOffersQueryHandler(IFundingOfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetFundingOffersQueryResponse>> Handle(GetFundingOffersQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetFundingOffersQueryResponse>();
            response.Success = false;
            try
            {
                var result = await _repository.GetFundingOffersAsync();
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
