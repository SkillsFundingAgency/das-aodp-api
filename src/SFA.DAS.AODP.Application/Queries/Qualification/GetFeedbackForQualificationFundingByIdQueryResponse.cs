using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetFeedbackForQualificationFundingByIdQueryResponse
{
    public Guid Id { get; set; }
    public Guid QualificationVersionId { get; set; }
    public string Status { get; set; }
    public string? Comments { get; set; }

    public static implicit operator GetFeedbackForQualificationFundingByIdQueryResponse(QualificationFundingFeedbacks feedback)
    {
        GetFeedbackForQualificationFundingByIdQueryResponse model = new()
        {
            Id = feedback.Id,
            QualificationVersionId = feedback.QualificationVersionId,
            Comments = feedback.Comments,
            Status = feedback.Status,
        };
        return model;
    }
}