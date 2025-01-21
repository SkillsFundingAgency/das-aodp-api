using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public record GetAllPagesQueryRequest(Guid SectionId) : IRequest<GetAllPagesQueryResponse>;