using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationSectionStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationSectionStatusByApplicationIdQuery, GetApplicationSectionStatusByApplicationIdQueryResponse>
{
    private readonly IApplicationPageRepository _applicationPageRepository;

    public GetApplicationSectionStatusByApplicationIdQueryHandler(IApplicationPageRepository applicationPageRepository)
    {
        _applicationPageRepository = applicationPageRepository;
    }

    public async Task<GetApplicationSectionStatusByApplicationIdQueryResponse> Handle(GetApplicationSectionStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationSectionStatusByApplicationIdQueryResponse();
        response.Success = false;
        try
        {
            List<ApplicationPage> result = await _applicationPageRepository.GetBySectionIdAsync(request.SectionId, request.ApplicationId);
            response = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
