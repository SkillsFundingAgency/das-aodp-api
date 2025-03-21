using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationFormStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationFormStatusByApplicationIdQuery, BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationReviewRepository _applicationReviewRepository;
    public GetApplicationFormStatusByApplicationIdQueryHandler(IApplicationRepository applicationRepository, IApplicationReviewRepository applicationReviewRepository)
    {
        _applicationRepository = applicationRepository;
        _applicationReviewRepository = applicationReviewRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>> Handle(GetApplicationFormStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>();
        try
        {
            Application result = await _applicationRepository.GetByIdAsync(request.ApplicationId);
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
            response.Value.ReviewExists = await _applicationReviewRepository.CheckIfReviewExistsByApplicationIdAsync(request.ApplicationId);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}