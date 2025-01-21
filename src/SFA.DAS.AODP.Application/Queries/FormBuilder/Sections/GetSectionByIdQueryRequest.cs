using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public record GetSectionByIdQueryRequest(Guid Id = default) : IRequest<GetSectionByIdQueryResponse>;