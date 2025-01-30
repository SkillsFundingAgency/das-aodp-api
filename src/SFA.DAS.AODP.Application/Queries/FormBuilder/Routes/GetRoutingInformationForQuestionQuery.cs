using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQuery : IRequest<GetRoutingInformationForQuestionQueryResponse>
    {
        public Guid FormVersionId { get; set; }
        public Guid QuestionId { get; set; }
    }
}