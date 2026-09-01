using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover;

[ExcludeFromCodeCoverage]
public sealed class RolloverFundingUpdate
{
    private readonly Action<DateOnly, string?, DateTime> _apply;

    public RolloverFundingUpdate(
        Guid id,
        string sourceType,
        Guid sourceQualificationId,
        Guid fundingOfferId,
        string academicYear,
        DateOnly? fundingApprovalEndDate,
        Action<DateOnly, string?, DateTime> apply)
    {
        Id = id;
        SourceType = sourceType;
        SourceQualificationId = sourceQualificationId;
        FundingOfferId = fundingOfferId;
        AcademicYear = academicYear;
        FundingApprovalEndDate = fundingApprovalEndDate;
        _apply = apply;
    }

    public Guid Id { get; }

    public string SourceType { get; }

    public Guid SourceQualificationId { get; }

    public Guid FundingOfferId { get; }

    public string AcademicYear { get; }

    public DateOnly? FundingApprovalEndDate { get; private set; }

    public string? Comments { get; private set; }

    public void ApplyFundingEndDate(DateOnly fundingApprovalEndDate, string? comments, DateTime updatedAt)
    {
        _apply(fundingApprovalEndDate, comments, updatedAt);
        FundingApprovalEndDate = fundingApprovalEndDate;
        Comments = comments;
    }
}

