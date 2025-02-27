using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessagesByIdQueryHandler : IRequestHandler<GetApplicationMessagesByIdQuery, BaseMediatrResponse<GetApplicationMessagesByIdQueryResponse>>
{
    private readonly IApplicationMessagesRepository _messagesRepository;

    public GetApplicationMessagesByIdQueryHandler(IApplicationMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationMessagesByIdQueryResponse>> Handle(GetApplicationMessagesByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationMessagesByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _messagesRepository.GetMessagesByApplicationIdAsync(request.ApplicationId);
            response.Value = new()
            {
                ApplicationId = request.ApplicationId,
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