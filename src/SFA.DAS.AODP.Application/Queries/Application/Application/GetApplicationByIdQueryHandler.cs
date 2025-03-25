using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, BaseMediatrResponse<GetApplicationByIdQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationByIdQueryHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationByIdQueryResponse>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            response.Value = new()
            {
                ApplicationId = result.Id,
                FormVersionId = result.FormVersionId,
                OrganisationId = result.OrganisationId,
                Reference = result.ReferenceId,
                Name = result.Name,
                Owner = result.Owner,
                QualificationNumber = result.QualificationNumber,
                Status = result.Status
            };
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