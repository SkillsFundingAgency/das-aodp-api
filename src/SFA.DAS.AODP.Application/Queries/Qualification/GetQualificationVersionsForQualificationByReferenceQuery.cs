using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetQualificationVersionsForQualificationByReferenceQuery : IRequest<BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>>
    {
        public string QualificationReference { get; set; }

        public GetQualificationVersionsForQualificationByReferenceQuery(string qualificationReference)
        {
            QualificationReference = qualificationReference;
        }
    }

}