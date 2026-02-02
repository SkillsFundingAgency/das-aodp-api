using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationsByQanQueryHandler : IRequestHandler<GetApplicationsByQanQuery, BaseMediatrResponse<GetApplicationsByQanQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationsByQanQueryHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }
    public async Task<BaseMediatrResponse<GetApplicationsByQanQueryResponse>> Handle(GetApplicationsByQanQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationsByQanQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _applicationRepository.GetByQan(request.Qan);
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
