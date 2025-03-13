using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetFeedbackForQualificationFundingByIdQuery : IRequest<BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>>
    {
        public Guid QualificationVersionId { get; set; }

        public GetFeedbackForQualificationFundingByIdQuery(Guid qualificationVersionId)
        {
            QualificationVersionId = qualificationVersionId;
        }
    }

}