using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableQuestionsForRoutingQuery : IRequest<BaseMediatrResponse<GetAvailableQuestionsForRoutingQueryResponse>>
    {
        public Guid PageId { get; set; }
    }
}