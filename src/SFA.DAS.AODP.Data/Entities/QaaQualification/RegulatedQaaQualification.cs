using SFA.DAS.AODP.Data.Extensions;
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

    public DateTime FirstSeenAt { get; private set; }

    public DateTime LastChangedAt { get; private set; }

    public string ContentHash { get; private set; } = null!;

    public QaaImportComparisonOutcome LatestImportComparisonOutcome { get; private set; }

    public QaaLastDateForRegistrationChangeType LastDateForRegistrationChangeType { get; private set; }

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

    public bool IsDiscontinued { get; private set; }

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

    public RegulatedQaaQualification SetFundingApprovalEndDate(DateTime publicationDate, IQaaFundingApprovalEndDateCalculator qaaFundingApprovalEndDateCalculator)
    {
        LastFundingApprovalEndDate = qaaFundingApprovalEndDateCalculator.CalculateFundingApprovalEndDate(LastDateForRegistration, LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate));
        
        return this;
    }
}

public interface IQaaFundingApprovalEndDateCalculator
{
    DateOnly? CalculateFundingApprovalEndDate(DateOnly lastDateForRegistration, DateOnly? currentFundingApprovalEndDate, DateOnly publicationDate);
}

public class QaaFundingApprovalEndDateCalculator(
    ISystemClockProvider clockProvider,
    IIlrSubmissionDeadlinesProvider ilrSubmissionDeadlinesProvider,
    IAcademicYearProvider academicYearProvider) : IQaaFundingApprovalEndDateCalculator
{
    private readonly ISystemClockProvider _clockProvider = clockProvider;
    private readonly IIlrSubmissionDeadlinesProvider _ilrSubmissionDeadlinesProvider = ilrSubmissionDeadlinesProvider;
    private readonly IAcademicYearProvider _academicYearProvider = academicYearProvider;

    public DateOnly? CalculateFundingApprovalEndDate(DateOnly lastDateForRegistration, DateOnly? currentFundingApprovalEndDate, DateOnly publicationDate)
    {
        var fundingApprovalEndDate = currentFundingApprovalEndDate;
        if (lastDateForRegistration > publicationDate)
        {
            var currentAcademicYear = _academicYearProvider.GetCurrentAcademicYearEndDate();
            var ilrFinalSubmissionDeadline = _ilrSubmissionDeadlinesProvider.GetFinalSubmissionDeadline();

            if (_clockProvider.Today >= ilrFinalSubmissionDeadline.Date)
            {
                currentAcademicYear = currentAcademicYear.AddYears(1);
            }

            var dates = new List<DateOnly>
            {
                lastDateForRegistration,
                currentAcademicYear
            };

            fundingApprovalEndDate = dates.Min();
        }
        else if (lastDateForRegistration < publicationDate)
        {
            if (lastDateForRegistration > currentFundingApprovalEndDate ||
                currentFundingApprovalEndDate is null)
            {
                fundingApprovalEndDate = lastDateForRegistration;
            }
        }

        return fundingApprovalEndDate;
    }
}

public interface IIlrSubmissionDeadlinesProvider
{
    IlrSubmissionDeadline GetFinalSubmissionDeadline();
}

public class IlrSubmissionDeadlinesProvider(ISystemClockProvider clock) : IIlrSubmissionDeadlinesProvider
{
    public IlrSubmissionDeadline GetFinalSubmissionDeadline()
    {
        var today = clock.Today;
        var startingDate = new DateTime(today.Year, 10, 1);
        var r02DeadlineDate = startingDate.GetSpecificWorkingDateOfMonth(startingDate.Year, startingDate.Month, 3);

        return new IlrSubmissionDeadline("R14", DateOnly.FromDateTime(r02DeadlineDate.AddDays(14).GetClosestDayOfWeek(DayOfWeek.Thursday)));
    }
}

public sealed record IlrSubmissionDeadline(string Period, DateOnly Date);