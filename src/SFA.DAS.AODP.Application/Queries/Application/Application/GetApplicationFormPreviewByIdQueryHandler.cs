using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationFormPreviewByIdQueryHandler : IRequestHandler<GetApplicationFormPreviewByIdQuery, BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationFormPreviewByIdQueryHandler(IQuestionRepository questionRepository, IApplicationRepository applicationRepository)
    {
        _questionRepository = questionRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>> Handle(GetApplicationFormPreviewByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>();
        response.Success = false;
        try
        {
            var formVersionId = await _applicationRepository.GetFormVersionIdForApplicationAsync(request.ApplicationId);
            var questions = await _questionRepository.GetQuestionsByFormVersionIdAsync(formVersionId);
            response.Value = new()
            {
                ApplicationId = request.ApplicationId,
                FormVersionId = formVersionId
            };
            response.Value.Data = [.. questions];
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