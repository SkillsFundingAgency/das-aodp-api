using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Offers
{
    public class GetFundingOffersQuery : IRequest<BaseMediatrResponse<GetFundingOffersQueryResponse>>
    {
    }
}
