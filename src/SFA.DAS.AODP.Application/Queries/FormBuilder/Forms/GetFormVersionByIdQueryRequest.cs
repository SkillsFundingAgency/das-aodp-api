using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQueryRequest : IRequest<GetFormVersionByIdQueryResponse>
{
    public Guid Id { get; set; }
}