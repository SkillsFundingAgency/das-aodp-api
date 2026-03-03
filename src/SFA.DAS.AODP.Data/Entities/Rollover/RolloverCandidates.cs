using System.ComponentModel.DataAnnotations.Schema;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("RolloverCandidates")]
public class RolloverCandidates
{
    public Guid Id { get; private set; }

    public Guid QualificationVersionId { get; private set; }

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

    public virtual QualificationVersions QualificationVersion { get; set; } = null!;

    public virtual RolloverDecisionRun DecisionRun { get; private set; } = null!;

    public static RolloverCandidates CreateInitialRound(
        Guid qualificationVersionId,
        Guid fundingOfferId,
        string academicYear,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new ArgumentNullException(nameof(academicYear));
        }

        return new RolloverCandidates
        {
            QualificationVersionId = qualificationVersionId,
            FundingOfferId = fundingOfferId,
            AcademicYear = academicYear,
            RolloverRound = 1,
            RolloverStatus = RolloverStatus.NeedsReview,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            IsActive = true
        };
    }
}