using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationFormByIdQueryHandler : IRequestHandler<GetApplicationFormByIdQuery, BaseMediatrResponse<GetApplicationFormByIdQueryResponse>>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public GetApplicationFormByIdQueryHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationFormByIdQueryResponse>> Handle(GetApplicationFormByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationFormByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _formVersionRepository.GetFormVersionByIdAsync(request.FormVersionId);
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