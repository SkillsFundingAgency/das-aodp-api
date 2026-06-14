using SFA.DAS.AODP.Data.Providers;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

[Table("QaaQualification", Schema = "regulated")]
public partial class RegulatedQaaQualification
{
    /// <summary>
    /// Gets the unique identifier for the instance.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The date which this snapshot of data was loaded.
    /// </summary>
    public DateTime DateOfDataSnapshot { get; private set; }

    /// <summary>
    /// When the qualification was first seen in the data feed.
    /// </summary>
    public DateTime FirstSeenAt { get; private set; }

    /// <summary>
    /// WHen the qualification was last changed.
    /// </summary>
    public DateTime LastChangedAt { get; private set; }

    /// <summary>
    /// A hash of the content for the qualification, this is used to determine if the content has changed since the last import, and therefore whether we need to update the record in the database or not.
    /// </summary>
    public string ContentHash { get; private set; } = null!;

    /// <summary>
    /// The outcome of the latest import comparison, this is used to determine whether the record was created, updated or unchanged as part of the latest import.
    /// </summary>
    public QaaImportComparisonOutcome LatestImportComparisonOutcome { get; private set; }

    /// <summary>
    /// The type of change that caused the last date for registration to change, this is used to determine whether we need to update the record in the database or not, and also to determine whether we need to update the last funding approval end date or not.
    /// </summary>
    public QaaLastDateForRegistrationChangeType LastDateForRegistrationChangeType { get; private set; }

    /// <summary>
    /// The unique identifier for the latest qualification history record for this qualification, this is used to link to the history records for the qualification, and to determine whether we need to create a new history record or not.
    /// </summary>
    public Guid? LatestQaaQualificationHistoryId { get; private set; }

    /// <summary>
    /// The unique learning AIM code for the qualification.
    /// </summary>
    public string AimCode { get; private set; } = null!;

    /// <summary>
    /// The qualification title.
    /// </summary>
    public string QualificationTitle { get; private set; } = null!;

    /// <summary>
    /// The awarding body (otherwise known as AVAs) that is delivery the qualification.
    /// </summary>
    public string AwardingBody { get; private set; } = null!;

    /// <summary>
    /// The level for the qualification, for QAA this is always Level 3.
    /// </summary>
    public string Level { get; private set; } = null!;

    /// <summary>
    /// The type of qualification, for QAA this is always 'Access to HE'.
    /// </summary>
    public string Type { get; private set; } = null!;

    /// <summary>
    /// The current status of the qualification, as we simply import this data, the status is always Approved.
    /// </summary>
    public string Status { get; private set; } = null!;

    /// <summary>
    /// When the qualification started.
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// When the last date for registration is.
    /// </summary>
    public DateOnly LastDateForRegistration { get; private set; }

    /// <summary>
    /// Whether the qualification is discontinued, this is determined by the Qaa API that returns this data directly.
    /// </summary>
    public bool IsDiscontinued { get; private set; }

    /// <summary>
    /// The date the qualification was discontinued, populated only if <see cref="IsDiscontinued"/> is true. The value comes from the Qaa API.
    /// </summary>
    public DateOnly? DiscontinuedDate { get; private set; }

    /// <summary>
    /// What date is the last date that funding can be approved for, this is set as part of the output file generation.
    /// </summary>
    public DateOnly? LastFundingApprovalEndDate { get; set; }

    /// <summary>
    /// A value object representation for the sector subject area.
    /// </summary>
    public SectorSubjectArea SectorSubjectArea { get; private set; } = null!;

    /// <summary>
    /// Creates a new entry.
    /// </summary>
    /// <returns>The newly created entry.</returns>
    public static RegulatedQaaQualification Create(
        DateTime dateOfDataSnapshot,
        string aimCode,
        string qualificationTitle,
        string awardingBody,
        DateOnly startDateForRegistration,
        DateOnly lastDateForRegistration,
        SectorSubjectArea sectorSubjectArea)
    {
        return new RegulatedQaaQualification
        {
            DateOfDataSnapshot = dateOfDataSnapshot,
            AimCode = aimCode,
            QualificationTitle = qualificationTitle,
            AwardingBody = awardingBody,
            Level = "Level 3",
            Type = "Access to Higher Education",
            Status = "Approved",
            StartDate = startDateForRegistration,
            LastDateForRegistration = lastDateForRegistration,
            SectorSubjectArea = sectorSubjectArea,
            ContentHash = "test"
        };
    }

    /// <summary>
    /// Set the funding approval end date for the Qaa qualification. This is only calculated and set at the point when an output file is generated as the calculations use the publication date of the output file.
    /// </summary>
    /// <param name="publicationDate">The date on which the output file will be published.</param>
    /// <param name="qaaFundingApprovalEndDateCalculator">Provides access to a calculator for determining the funding approval end date.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns>The updated regulated qualification.</returns>
    public async Task<RegulatedQaaQualification> SetFundingApprovalEndDateAsync(DateTime publicationDate, IQaaFundingApprovalEndDateCalculator qaaFundingApprovalEndDateCalculator, CancellationToken cancellationToken)
    {
        LastFundingApprovalEndDate = await qaaFundingApprovalEndDateCalculator.CalculateFundingApprovalEndDateAsync(AimCode, LastDateForRegistration, LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate), cancellationToken);
        
        return this;
    }
}