using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessageByIdQuery : IRequest<BaseMediatrResponse<GetApplicationMessageByIdQueryResponse>>
{
    public Guid MessageId { get; set; }

    public GetApplicationMessageByIdQuery(Guid messageId)
    {
        MessageId = messageId;
    }
}
