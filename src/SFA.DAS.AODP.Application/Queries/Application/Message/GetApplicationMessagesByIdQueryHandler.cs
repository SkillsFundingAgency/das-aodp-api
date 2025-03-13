using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

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