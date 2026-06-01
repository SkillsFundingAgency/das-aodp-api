using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

[ExcludeFromCodeCoverage]
[Table("QaaQualificationHistory", Schema = "regulated")]
public partial class RegulatedQaaQualificationHistory
{
    public Guid Id { get; private set; }

    public Guid QaaQualificationId { get; private set; }

    public string AimCode { get; private set; } = null!;

    public DateTime DateOfDataSnapshot { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public string ContentHash { get; private set; } = null!;

    public string QualificationTitle { get; private set; } = null!;

    public string AwardingBody { get; private set; } = null!;

    public DateOnly StartDate { get; private set; }

    public DateOnly LastDateForRegistration { get; private set; }

    public bool IsDiscontinued { get; private set; }

    public DateOnly? DiscontinuedDate { get; private set; }

    public SectorSubjectArea SectorSubjectArea { get; private set; } = null!;

    public string LastDateForRegistrationChangeType { get; private set; } = null!;
}
