using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public record GetPageByIdQueryRequest(Guid Id) : IRequest<GetPageByIdQueryResponse>;