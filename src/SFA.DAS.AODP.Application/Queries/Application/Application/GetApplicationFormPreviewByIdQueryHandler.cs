using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationFormPreviewByIdQueryHandler : IRequestHandler<GetApplicationFormPreviewByIdQuery, BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationFormPreviewByIdQueryHandler(ISectionRepository sectionRepository, IApplicationRepository applicationRepository)
    {
        _sectionRepository = sectionRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>> Handle(GetApplicationFormPreviewByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>();
        response.Success = false;
        try
        {
            var formVersionId = await _applicationRepository.GetFormVersionIdForApplicationAsync(request.ApplicationId);
            var sections = await _sectionRepository.GetSectionsWithPagesAndQuestionsByFormVersionIdAsync(formVersionId);

            response.Value = GetApplicationFormPreviewByIdQueryResponse.Map(request.ApplicationId, formVersionId, sections);
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