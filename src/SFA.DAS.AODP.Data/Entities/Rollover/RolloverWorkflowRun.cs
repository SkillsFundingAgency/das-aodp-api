using System.ComponentModel.DataAnnotations.Schema;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("RolloverWorkflowRun")]
public class RolloverWorkflowRun
{
    public Guid Id { get; private set; }

    public string AcademicYear { get; private set; } = null!;

    public SelectionMethod SelectionMethod { get; private set; }

    public DateTime? FundingEndDateEligibilityThreshold { get; private set; }

    public DateTime? OperationalEndDateEligibilityThreshold { get; private set; }

    public DateTime? MaximumApprovalFundingEndDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedByUsername { get; private set; } = null!;

    public virtual IReadOnlyCollection<RolloverWorkflowRunFundingOffer> FundingOffers => _fundingOffers;
    private readonly List<RolloverWorkflowRunFundingOffer> _fundingOffers = [];

    public virtual IReadOnlyCollection<RolloverWorkflowRunFilter> Filters => _filters;
    private readonly List<RolloverWorkflowRunFilter> _filters = [];

    public virtual IReadOnlyCollection<RolloverWorkflowCandidate> Candidates => _candidates;
    private readonly List<RolloverWorkflowCandidate> _candidates = [];

    public static RolloverWorkflowRun Create(
        string academicYear,
        SelectionMethod selectionMethod,
        DateTime? fundingEndDateEligibilityThreshold,
        DateTime? operationalEndDateEligibilityThreshold,
        DateTime? maximumApprovalFundingEndDate,
        string createdByUsername,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new ArgumentNullException(nameof(academicYear));
        }

        if (string.IsNullOrWhiteSpace(createdByUsername))
        {
            throw new ArgumentNullException(nameof(createdByUsername));
        }

        return new RolloverWorkflowRun
        {
            AcademicYear = academicYear,
            SelectionMethod = selectionMethod,
            FundingEndDateEligibilityThreshold = fundingEndDateEligibilityThreshold,
            OperationalEndDateEligibilityThreshold = operationalEndDateEligibilityThreshold,
            MaximumApprovalFundingEndDate = maximumApprovalFundingEndDate,
            CreatedByUsername = createdByUsername,
            CreatedAt = createdAt
        };
    }
}