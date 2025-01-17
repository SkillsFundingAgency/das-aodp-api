using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormByVersionIdQuery : IRequest<GetFormByVersionIdQueryResponse>
{
    public Guid Id { get; set; }
}
