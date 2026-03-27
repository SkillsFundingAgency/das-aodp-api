namespace SFA.DAS.AODP.Data.Entities.Rollover;

public class RolloverWorkflowCandidatesP1Checks
{
    public Guid WorkflowCandidateId { get; set; }
    public Guid QualificationVersionId { get; set; }
    public Guid FundingOfferId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public bool IncludedInP1Export { get; set; }
    public bool IncludedInFinalUpload { get; set; }
    public DateTime CurrentFundingEndDate { get; set; }
    public DateTime? ProposedFundingEndDate { get; set; }

    public string? FundingStream { get; set; }
    public int? RolloverRound { get; set; }
    public DateTime? ThresholdDate { get; set; }
    public DateTime? LatestFundingApprovalEndDate { get; set; }
    public DateTime? OperationalStartDate { get; set; }
    public DateTime? OperationalEndDate { get; set; }
    public bool OfferedInEngland { get; set; }
    public int? Glh { get; set; }
    public int? Tqt { get; set; }
    public bool IsOnDefundingList { get; set; }
    public DateTime? Age1416 { get; set; }
    public DateTime? Age1619 { get; set; }
    public DateTime? LocalFlexibilities { get; set; }
    public DateTime? LegalEntitlementL2L3 { get; set; }
    public DateTime? LegalEntitlementEnglishandMaths { get; set; }
    public DateTime? DigitalEntitlement { get; set; }
    public DateTime? ESFL3L4 { get; set; }
    public DateTime? AdvancedLearnerLoans { get; set; }
    public DateTime? LifelongLearningEntitlement { get; set; }
    public DateTime? L3FreeCoursesForJobs { get; set; }
    public DateTime? CoF { get; set; }

    public DateTime? GetPldnsDate()
    {
        if (string.IsNullOrWhiteSpace(FundingStream))
        {
            return null;
        }

        return FundingStream.Trim() switch
        {
            nameof(Age1416) => Age1416,
            nameof(Age1619) => Age1619,
            nameof(LocalFlexibilities) => LocalFlexibilities,
            nameof(LegalEntitlementL2L3) => LegalEntitlementL2L3,
            nameof(LegalEntitlementEnglishandMaths) => LegalEntitlementEnglishandMaths,
            nameof(DigitalEntitlement) => DigitalEntitlement,
            nameof(ESFL3L4) => ESFL3L4,
            nameof(AdvancedLearnerLoans) => AdvancedLearnerLoans,
            nameof(LifelongLearningEntitlement) => LifelongLearningEntitlement,
            nameof(L3FreeCoursesForJobs) => L3FreeCoursesForJobs,
            nameof(CoF) => CoF,
            _ => null
        };
    }

    public (DateTime? AcademicYearStartDate, DateTime? AcademicYearEndDate) GetAcademicYearDates()
    {
        if (string.IsNullOrWhiteSpace(AcademicYear))
        {
            return (default, default);
        }

        var parts = AcademicYear.Split('/');
        if (parts.Length != 2 || !int.TryParse(parts[0], out var startYear))
        {
            throw new ArgumentException("Academic year must be in the format 'YYYY/YY'.", nameof(AcademicYear));
        }

        var startDate = new DateTime(startYear, 8, 1);
        var endDate = new DateTime(startYear + 1, 7, 31);

        return (startDate, endDate);
    }
}
