using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessageByIdQueryHandler : IRequestHandler<GetApplicationMessageByIdQuery, BaseMediatrResponse<GetApplicationMessageByIdQueryResponse>>
{
    private readonly IApplicationMessagesRepository _messagesRepository;

    public GetApplicationMessageByIdQueryHandler(IApplicationMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationMessageByIdQueryResponse>> Handle(GetApplicationMessageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationMessageByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _messagesRepository.GetByIdAsync(request.MessageId);
            response.Value = result;
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