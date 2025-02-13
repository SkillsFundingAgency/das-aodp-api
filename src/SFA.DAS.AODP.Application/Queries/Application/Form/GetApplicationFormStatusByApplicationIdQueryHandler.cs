using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationFormStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationFormStatusByApplicationIdQuery, BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationPageRepository _applicationPageRepository;
    public GetApplicationFormStatusByApplicationIdQueryHandler(IApplicationRepository applicationRepository, IApplicationPageRepository applicationPageRepository)
    {
        this._applicationRepository = applicationRepository;
        _applicationPageRepository = applicationPageRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>> Handle(GetApplicationFormStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>();
        try
        {
            Application result = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            var pages = await _applicationPageRepository.GetApplicationPagesByApplicationIdAsync(request.ApplicationId);

            response.Value = result;

            var sectionSummary = await _applicationRepository.GetSectionSummaryByApplicationIdAsync(request.ApplicationId);

            foreach (var section in sectionSummary ?? [])
            {
                response.Value.Sections.Add(new()
                {
                    SectionId = section.SectionId,
                    PagesRemaining = section.RemainingPages ?? 0,
                    SkippedPages = section.SkippedPages ?? 0,
                    TotalPages = section.TotalPages ?? 0,
                });
            }
            response.Value.ReadyForSubmit = !response.Value.Sections.Any(a => a.PagesRemaining > 0);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}