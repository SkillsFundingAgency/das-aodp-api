using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationFormsQueryHandler : IRequestHandler<GetApplicationFormsQuery, BaseMediatrResponse< GetApplicationFormsQueryResponse>>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public GetApplicationFormsQueryHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormsQueryResponse>> Handle(GetApplicationFormsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormsQueryResponse>();
        try
        {
            var result = await _formVersionRepository.GetPublishedFormVersions();
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
