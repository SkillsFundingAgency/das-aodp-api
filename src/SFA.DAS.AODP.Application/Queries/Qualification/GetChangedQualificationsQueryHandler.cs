using Azure.Core;
using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryHandler(IQualificationsRepository qualificationsRepository) : IRequestHandler<GetChangedQualificationsQuery, BaseMediatrResponse<GetChangedQualificationsQueryResponse>>
{
    private readonly IQualificationsRepository _qualificationsRepository = qualificationsRepository;

    public async Task<BaseMediatrResponse<GetChangedQualificationsQueryResponse>> Handle(GetChangedQualificationsQuery query, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetChangedQualificationsQueryResponse>();
        response.Success = false;

        try
        {
            var changedQualifications = await _qualificationsRepository.GetChangedQualificationsAsync();
            response.Value.Data = [.. changedQualifications];
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }

}
