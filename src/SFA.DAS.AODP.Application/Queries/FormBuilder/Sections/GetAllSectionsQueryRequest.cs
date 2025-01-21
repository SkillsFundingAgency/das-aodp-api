using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public record GetAllSectionsQueryRequest(Guid FormId) : IRequest<GetAllSectionsQueryResponse>;
