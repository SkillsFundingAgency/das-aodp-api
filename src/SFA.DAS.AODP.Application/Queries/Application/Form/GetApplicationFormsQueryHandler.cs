using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationFormsQueryHandler : IRequestHandler<GetApplicationFormsQuery, GetApplicationFormsQueryResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public GetApplicationFormsQueryHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<GetApplicationFormsQueryResponse> Handle(GetApplicationFormsQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationFormsQueryResponse();
        response.Success = false;
        try
        {
            var result = await _formVersionRepository.GetPublishedFormVersions();
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
