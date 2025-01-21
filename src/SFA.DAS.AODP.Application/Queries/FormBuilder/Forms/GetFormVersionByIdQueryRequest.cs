using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public record GetFormVersionByIdQueryRequest(Guid Id) : IRequest<GetFormVersionByIdQueryResponse>;