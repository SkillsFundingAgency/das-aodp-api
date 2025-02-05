using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationFormByIdQueryHandler : IRequestHandler<GetApplicationFormByIdQuery, GetApplicationFormByIdQueryResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public GetApplicationFormByIdQueryHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<GetApplicationFormByIdQueryResponse> Handle(GetApplicationFormByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationFormByIdQueryResponse();
        response.Success = false;
        try
        {
            var result = await _formVersionRepository.GetFormVersionByIdAsync(request.FormVersionId);
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