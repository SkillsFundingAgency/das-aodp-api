using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationMetadataByIdQueryHandler : IRequestHandler<GetApplicationMetadataByIdQuery, BaseMediatrResponse<GetApplicationMetadataByIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationMetadataByIdQueryHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationMetadataByIdQueryResponse>> Handle(GetApplicationMetadataByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationMetadataByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _applicationRepository.GetApplicationMetadataById(request.ApplicationId);
            response.Value = new()
            {
                ApplicationId = result.Id,
                FormVersionId = result.FormVersionId,
                OrganisationId = result.OrganisationId
            };
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}