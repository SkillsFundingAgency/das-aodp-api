using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQuery : IRequest<BaseMediatrResponse<GetRoutingInformationForQuestionQueryResponse>>
    {
        public Guid FormVersionId { get; set; }
        public Guid QuestionId { get; set; }
    }
}