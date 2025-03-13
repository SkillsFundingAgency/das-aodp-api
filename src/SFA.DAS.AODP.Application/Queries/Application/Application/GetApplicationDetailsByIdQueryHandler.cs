using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using static SFA.DAS.AODP.Data.Repositories.Application.ApplicationQuestionAnswerRepository;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationDetailsByIdQueryHandler : IRequestHandler<GetApplicationDetailsByIdQuery, BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>>
{
    private readonly IApplicationQuestionAnswerRepository _applicationQuestionAnswerRepository;

    public GetApplicationDetailsByIdQueryHandler(IApplicationQuestionAnswerRepository applicationQuestionAnswerRepository)
    {
        _applicationQuestionAnswerRepository = applicationQuestionAnswerRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>> Handle(GetApplicationDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>();
        response.Success = false;
        try
        {
            List<ApplicationQuestionAnswersDTO> result = await _applicationQuestionAnswerRepository.GetAnswersByApplicationId(request.ApplicationId);
            response.Value = GetApplicationDetailsByIdQueryResponse.Map(request.ApplicationId, result);
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