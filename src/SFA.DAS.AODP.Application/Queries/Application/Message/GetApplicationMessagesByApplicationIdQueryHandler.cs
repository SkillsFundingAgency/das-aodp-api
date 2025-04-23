using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessagesByApplicationIdQueryHandler : IRequestHandler<GetApplicationMessagesByApplicationIdQuery, BaseMediatrResponse<GetApplicationMessagesByApplicationIdQueryResponse>>
{
    private readonly IApplicationMessagesRepository _messagesRepository;

    public GetApplicationMessagesByApplicationIdQueryHandler(IApplicationMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationMessagesByApplicationIdQueryResponse>> Handle(GetApplicationMessagesByApplicationIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationMessagesByApplicationIdQueryResponse>();
        response.Success = false;
        try
        {
            if (!Enum.TryParse(request.UserType, out UserType userType)) throw new Exception("Invalid user type provided");

            List<SFA.DAS.AODP.Data.Entities.Application.Message> result = await _messagesRepository.GetMessagesByApplicationIdAndUserTypeAsync(request.ApplicationId, userType);
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
