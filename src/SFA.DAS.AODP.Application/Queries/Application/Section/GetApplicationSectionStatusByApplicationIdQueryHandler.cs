using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationSectionStatusByApplicationIdQueryHandler : IRequestHandler<GetApplicationSectionStatusByApplicationIdQuery, BaseMediatrResponse<GetApplicationSectionStatusByApplicationIdQueryResponse>>
{
    private readonly IApplicationPageRepository _applicationPageRepository;

    public GetApplicationSectionStatusByApplicationIdQueryHandler(IApplicationPageRepository applicationPageRepository)
    {
        _applicationPageRepository = applicationPageRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationSectionStatusByApplicationIdQueryResponse>> Handle(GetApplicationSectionStatusByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationSectionStatusByApplicationIdQueryResponse>();
        response.Success = false;
        try
        {
            List<ApplicationPage> result = await _applicationPageRepository.GetBySectionIdAsync(request.SectionId, request.ApplicationId);
            response.Value = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
