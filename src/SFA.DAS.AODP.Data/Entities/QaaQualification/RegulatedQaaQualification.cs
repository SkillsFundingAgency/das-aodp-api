using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

[Table("QaaQualification", Schema = "regulated")]
public class RegulatedQaaQualification
{
    public Guid Id { get; private set; }

    public string AimCode { get; private set; }

    public string QualificationTitle { get; private set; }

    public string AwardingBody { get; private set; }

    public string Level { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public DateTime StartDate { get; private set; }

    public DateTime LastDateForRegistration { get; private set; }

    public DateTime? LastFundingApprovalEndDate { get; private set; }

    public SectorSubjectArea SectorSubjectArea { get; private set; } = null!;
}