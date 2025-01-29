using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableSectionsAndPagesForRoutingQuery : IRequest<GetAvailableSectionsAndPagesForRoutingQueryResponse>
    {
        public Guid FormVersionId { get; set; }
    }
}