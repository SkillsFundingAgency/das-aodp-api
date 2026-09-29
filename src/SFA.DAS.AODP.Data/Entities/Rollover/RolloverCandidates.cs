using System.ComponentModel.DataAnnotations.Schema;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("RolloverCandidates")]
public class RolloverCandidates
{
    public Guid Id { get; private set; }

    public string SourceType { get; private set; } = null!;

    public Guid SourceQualificationId { get; private set; }

    public Guid FundingOfferId { get; private set; }
    
    public string AcademicYear { get; private set; } = null!;

    public int RolloverRound { get; private set; }

    public Guid? RolloverDecisionRunId { get; private set; }
    
    public RolloverStatus RolloverStatus { get; private set; }
    
    public string? ExclusionReason { get; private set; }

    public DateTime? PreviousFundingEndDate { get; private set; }
    
    public DateTime? NewFundingEndDate { get; private set; }

    public DateTime? ReviewedAt { get; private set; }
    
    public string? ReviewedByUsername { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }

    public virtual RolloverDecisionRun RolloverDecisionRun { get; private set; } = null!;

    public virtual FundingOffer FundingOffer { get; set; } = null!;

    [NotMapped]
    public string SourceQualificationReference { get; private set; } = string.Empty;

    [NotMapped]
    public Guid? DiscussionQualificationId { get; private set; }

    public static RolloverCandidates CreateInitialRound(
        string sourceType,
        Guid sourceQualificationId,
        Guid fundingOfferId,
        string academicYear,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(sourceType))
        {
            throw new ArgumentNullException(nameof(sourceType));
        }

        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new ArgumentNullException(nameof(academicYear));
        }

        return new RolloverCandidates
        {
            Id = Guid.NewGuid(),
            SourceType = sourceType,
            SourceQualificationId = sourceQualificationId,
            FundingOfferId = fundingOfferId,
            AcademicYear = academicYear,
            RolloverRound = 1,
            RolloverStatus = RolloverStatus.NeedsReview,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            IsActive = true
        };
    }

    public void SetExtended(DateTime fundingEndDate)
    {
        RolloverStatus = RolloverStatus.Extended;
        NewFundingEndDate = fundingEndDate;
    }

    public void SetExcluded(string exclusionReason)
    {
        RolloverStatus = RolloverStatus.Excluded;
        ExclusionReason = exclusionReason;
    }

    public void RefreshFunding(DateOnly? fundingEndDate, DateTime updatedAt)
    {
        PreviousFundingEndDate = fundingEndDate?.ToDateTime(TimeOnly.MinValue);
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTime updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void Reactivate(DateOnly? fundingEndDate, DateTime updatedAt)
    {
        IsActive = true;
        RolloverStatus = RolloverStatus.NeedsReview;
        ExclusionReason = null;
        PreviousFundingEndDate = fundingEndDate?.ToDateTime(TimeOnly.MinValue);
        NewFundingEndDate = null;
        RolloverDecisionRunId = null;
        ReviewedAt = null;
        ReviewedByUsername = null;
        UpdatedAt = updatedAt;
    }

    public void MoveSourceQualification(Guid sourceQualificationId, DateTime updatedAt)
    {
        SourceQualificationId = sourceQualificationId;
        UpdatedAt = updatedAt;
    }

    public void SetSourceContext(string sourceQualificationReference, Guid? discussionQualificationId)
    {
        SourceQualificationReference = sourceQualificationReference;
        DiscussionQualificationId = discussionQualificationId;
    }
}
