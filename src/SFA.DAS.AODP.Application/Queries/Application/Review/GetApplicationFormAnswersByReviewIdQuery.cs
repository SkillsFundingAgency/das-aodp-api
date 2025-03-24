using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Review;
public class GetApplicationFormAnswersByReviewIdQuery : IRequest<BaseMediatrResponse<GetApplicationFormAnswersByReviewIdQueryResponse>>
{
    public GetApplicationFormAnswersByReviewIdQuery(Guid applicationReviewId)
    {
        ApplicationReviewId = applicationReviewId;
    }
    public Guid ApplicationReviewId { get; set; }
}
