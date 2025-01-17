using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryRequest : IRequest<GetAllPagesQueryResponse>
{
    public Guid SectionId { get; set; }
}