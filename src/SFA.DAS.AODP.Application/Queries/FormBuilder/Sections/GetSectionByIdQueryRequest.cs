using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryRequest : IRequest<GetSectionByIdQueryResponse>
{
    public Guid Id { get; set; }
}