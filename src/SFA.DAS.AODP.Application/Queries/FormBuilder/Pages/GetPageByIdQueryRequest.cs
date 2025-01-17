using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPageByIdQueryRequest : IRequest<GetPageByIdQueryResponse>
{
    public Guid Id { get; set; }
}