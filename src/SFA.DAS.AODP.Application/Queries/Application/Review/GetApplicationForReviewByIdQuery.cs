using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationForReviewByIdQuery : IRequest<BaseMediatrResponse<GetApplicationForReviewByIdQueryResponse>>
    {
        public Guid ApplicationReviewId { get; set; }

        public GetApplicationForReviewByIdQuery(Guid applicationReviewId)
        {
            ApplicationReviewId = applicationReviewId;
        }
    }

}