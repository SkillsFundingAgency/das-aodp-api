using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[ExcludeFromCodeCoverage]
[Table("RolloverDecisionRun")]
public class RolloverDecisionRun
{
    private readonly List<RolloverCandidates> _candidateRows = [];

    public Guid Id { get; private set; } 

    public string AcademicYear { get; private set; } = null!;

    public SelectionMethod SelectionMethod { get; private set; }

    public DateTime? FundingEndDateEligibilityThreshold { get; private set; }
    
    public DateTime? OperationalEndDateEligibilityThreshold { get; private set; }
    
    public DateTime? MaximumApprovalFundingEndDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedByUsername { get; private set; } = null!;

    public DateTime? CompletedAt { get; private set; }

    public virtual IReadOnlyCollection<RolloverCandidates> CandidateRows => _candidateRows;
}