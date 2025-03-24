using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review;

public class GetApplicationFormAnswersByReviewIdQueryHandler : IRequestHandler<GetApplicationFormAnswersByReviewIdQuery, BaseMediatrResponse<GetApplicationFormAnswersByReviewIdQueryResponse>>
{
    private readonly IApplicationQuestionAnswerRepository _applicationQuestionAnswerRepository;
    private readonly IApplicationReviewRepository _applicationReviewRepository;

    public GetApplicationFormAnswersByReviewIdQueryHandler(IApplicationQuestionAnswerRepository applicationQuestionAnswerRepository, IApplicationReviewRepository applicationReviewRepository)
    {
        _applicationQuestionAnswerRepository = applicationQuestionAnswerRepository;
        _applicationReviewRepository = applicationReviewRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormAnswersByReviewIdQueryResponse>> Handle(GetApplicationFormAnswersByReviewIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormAnswersByReviewIdQueryResponse>();
        response.Success = false;
        try
        {
            var applicationReview = await _applicationReviewRepository.GetByIdAsync(request.ApplicationReviewId);
            var applicationId = applicationReview.ApplicationId;
            var result = await _applicationQuestionAnswerRepository.GetAnswersByApplicationId(applicationId);

            response.Value = GetApplicationFormAnswersByReviewIdQueryResponse.Map(applicationId, result);
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