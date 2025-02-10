using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationFormStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationFormStatusByApplicationIdQuery, BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationFormStatusByApplicationIdQueryHandler(IApplicationRepository applicationRepository)
    {
        this._applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>> Handle(GetApplicationFormStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse>();
        try
        {
            Application result = await _applicationRepository.GetByIdAsync(request.ApplicationId);

            var remainingPagesBySections = await _applicationRepository.GetRemainingPagesBySectionForApplicationsAsync(request.ApplicationId);

            response.Value = result;

            foreach (var section in remainingPagesBySections ?? [])
            {
                response.Value.Sections.Add(new()
                {
                    SectionId = section.SectionId,
                    PagesRemaining = section.PageCount
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