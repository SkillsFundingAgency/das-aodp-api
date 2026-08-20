using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[ExcludeFromCodeCoverage]
public class AllQualificationFunding
{
    public Guid FundingId { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid SourceQualificationId { get; set; }

    public string QualificationReference { get; set; } = null!;

    public Guid FundingOfferId { get; set; }

    public string FundingStreamName { get; set; } = null!;

    public DateOnly? FundingApprovalStartDate { get; set; }

    public DateOnly? FundingApprovalEndDate { get; set; }

    public string? FundingStatus { get; set; }

    public string? QualificationName { get; set; }

    public string? Level { get; set; }

    public string? AwardingOrganisationName { get; set; }
}
