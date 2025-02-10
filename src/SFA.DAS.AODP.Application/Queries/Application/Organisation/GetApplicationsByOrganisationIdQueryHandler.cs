using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationsByOrganisationIdQueryHandler : IRequestHandler<GetApplicationsByOrganisationIdQuery, BaseMediatrResponse< GetApplicationsByOrganisationIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationsByOrganisationIdQueryHandler(IApplicationRepository applicationRepository)
    {
        this._applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationsByOrganisationIdQueryResponse>> Handle(GetApplicationsByOrganisationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationsByOrganisationIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _applicationRepository.GetByOrganisationId(request.OrganisationId);
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
