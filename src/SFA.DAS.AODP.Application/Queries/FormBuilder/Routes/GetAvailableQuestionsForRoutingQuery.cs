using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableQuestionsForRoutingQuery : IRequest<GetAvailableQuestionsForRoutingQueryResponse>
    {
        public Guid PageId { get; set; }
    }
}