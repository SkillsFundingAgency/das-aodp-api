using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationFormStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationFormStatusByApplicationIdQuery, GetApplicationFormStatusByApplicationIdQueryResponse>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationFormStatusByApplicationIdQueryHandler(IApplicationRepository applicationRepository)
    {
        this._applicationRepository = applicationRepository;
    }

    public async Task<GetApplicationFormStatusByApplicationIdQueryResponse> Handle(GetApplicationFormStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationFormStatusByApplicationIdQueryResponse();
        response.Success = false;
        try
        {
            Application result = await _applicationRepository.GetByIdAsync(request.ApplicationId);

            var remainingPagesBySections = await _applicationRepository.GetRemainingPagesBySectionForApplicationsAsync(request.ApplicationId);

            response = result;

            foreach (var section in remainingPagesBySections ?? [])
            {
                response.Sections.Add(new()
                {
                    SectionId = section.SectionId,
                    PagesRemaining = section.PageCount
                });
            }
            response.ReadyForSubmit = !response.Sections.Any(a => a.PagesRemaining > 0);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}