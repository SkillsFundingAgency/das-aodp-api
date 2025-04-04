using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class SaveSurveyCommandHandler : IRequestHandler<SaveSurveyCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly ISurveyRepository _surveyRepositoryRepository;

    public SaveSurveyCommandHandler(ISurveyRepository surveyRepositoryRepository)
    {
        _surveyRepositoryRepository = surveyRepositoryRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveSurveyCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
           await _surveyRepositoryRepository.Create(new()
            {
               Page = request.Page,
               SatisfactionScore = request.SatisfactionScore,
               Comments = request.Comments,
               Timestamp = DateTime.Now
           });

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.InnerException = ex;
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }
        return response;
    }
}
