using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using static SFA.DAS.AODP.Data.Repositories.Application.ApplicationQuestionAnswerRepository;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationDetailsByIdQueryHandler : IRequestHandler<GetApplicationDetailsByIdQuery, BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>>
{
    private readonly IApplicationQuestionAnswerRepository _applicationQuestionAnswerRepository;
    private readonly IApplicationReviewRepository _applicationReviewRepository;
    
    public GetApplicationDetailsByIdQueryHandler(IApplicationQuestionAnswerRepository applicationQuestionAnswerRepository, IApplicationReviewRepository applicationReviewRepository)
    {
        _applicationQuestionAnswerRepository = applicationQuestionAnswerRepository;
        _applicationReviewRepository = applicationReviewRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>> Handle(GetApplicationDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>();
        response.Success = false;
        try
        {
            var applicationReview = _applicationReviewRepository.GetByIdAsync(request.ApplicationReviewId);
            var applicationId = applicationReview.Result.ApplicationId;
            List<ApplicationQuestionAnswersDTO> result = await _applicationQuestionAnswerRepository.GetAnswersByApplicationId(applicationId);
            response.Value = GetApplicationDetailsByIdQueryResponse.Map(applicationId, result);
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