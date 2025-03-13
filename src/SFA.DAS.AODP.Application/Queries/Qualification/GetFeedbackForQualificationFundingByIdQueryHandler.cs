using MediatR;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetFeedbackForQualificationFundingByIdQueryHandler : IRequestHandler<GetFeedbackForQualificationFundingByIdQuery, BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>>
{
    private readonly IQualificationFundingFeedbackRepository _qualificationFundingFeedbackRepository;

    public GetFeedbackForQualificationFundingByIdQueryHandler(IQualificationFundingFeedbackRepository qualificationFundingFeedbackRepository)
    {
        _qualificationFundingFeedbackRepository = qualificationFundingFeedbackRepository;
    }

    public async Task<BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>> Handle(GetFeedbackForQualificationFundingByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _qualificationFundingFeedbackRepository.GetQualificationFundingFeedbackDetailsByIdAsync(request.QualificationVersionId);
            response.Value = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.InnerException = ex;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}