using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification
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