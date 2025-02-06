using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationsByOrganisationIdQueryHandler : IRequestHandler<GetApplicationsByOrganisationIdQuery, GetApplicationsByOrganisationIdQueryResponse>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationsByOrganisationIdQueryHandler(IApplicationRepository applicationRepository)
    {
        this._applicationRepository = applicationRepository;
    }

    public async Task<GetApplicationsByOrganisationIdQueryResponse> Handle(GetApplicationsByOrganisationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationsByOrganisationIdQueryResponse();
        response.Success = false;
        try
        {
            var result = await _applicationRepository.GetByOrganisationId(request.OrganisationId);
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
