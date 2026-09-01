using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

internal class RolloverCandidateSourceProjection
{
    public Guid CandidateId { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid SourceQualificationId { get; set; }

    public Guid FundingOfferId { get; set; }

    public string FundingStreamName { get; set; } = null!;

    public string QualificationReference { get; set; } = null!;

    public string? QualificationTitle { get; set; }

    public string? AwardingOrganisation { get; set; }

    public string? QualificationLevel { get; set; }

    public string? QualificationType { get; set; }

    public string? SectorSubjectArea { get; set; }

    public Guid AwardingOrganisationId { get; set; }

    public string? AwardingOrganisationFilterId { get; set; }

    public int? AwardingOrganisationUkprn { get; set; }

    public string? AwardingOrganisationRecognitionNumber { get; set; }

    public string? AwardingOrganisationNameLegal { get; set; }

    public string? AwardingOrganisationNameGovUk { get; set; }

    public string? AwardingOrganisationNameDsi { get; set; }

    public string? AwardingOrganisationAcronym { get; set; }

    public int RolloverRound { get; set; }

    public DateTime? PreviousFundingEndDate { get; set; }

    public DateTime? NewFundingEndDate { get; set; }

    public string AcademicYear { get; set; } = null!;

    public RolloverStatus RolloverStatus { get; set; }

    public Guid? DiscussionQualificationId { get; set; }

    public DateTime? OperationalStartDate { get; set; }

    public DateTime? OperationalEndDate { get; set; }

    public bool OfferedInEngland { get; set; }

    public bool IntentionToSeekFundingInEngland { get; set; }
}
