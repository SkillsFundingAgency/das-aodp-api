using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetDiscussionHistoriesForQualificationQueryHandler(IQualificationDetailsRepository repository) : IRequestHandler<GetDiscussionHistoriesForQualificationQuery, BaseMediatrResponse<GetDiscussionHistoriesForQualificationQueryResponse>>
{
    private readonly IQualificationDetailsRepository _repository = repository;

    public async Task<BaseMediatrResponse<GetDiscussionHistoriesForQualificationQueryResponse>> Handle(GetDiscussionHistoriesForQualificationQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetDiscussionHistoriesForQualificationQueryResponse>();
        try
        {
            var discussionHistories = await _repository.GetDiscussionHistoriesForQualificationRef(request.QualificationReference);
            response.Value.QualificationDiscussionHistories = [..discussionHistories];
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
