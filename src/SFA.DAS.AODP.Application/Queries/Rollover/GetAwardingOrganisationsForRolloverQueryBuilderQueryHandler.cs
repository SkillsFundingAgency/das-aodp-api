using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetAwardingOrganisationsForRolloverQueryBuilderQueryHandler(IRolloverRepository repository)
    : IRequestHandler<GetAwardingOrganisationsForRolloverQueryBuilderQuery, BaseMediatrResponse<GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse>> Handle(
        GetAwardingOrganisationsForRolloverQueryBuilderQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse>();

        try
        {
            response.Value.AwardingOrganisations =
                await repository.GetAwardingOrganisationsForRolloverQueryBuilderAsync(request.Filters, cancellationToken);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }
}
