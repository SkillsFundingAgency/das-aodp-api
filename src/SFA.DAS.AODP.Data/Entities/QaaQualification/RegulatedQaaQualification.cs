using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

[ExcludeFromCodeCoverage]
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
    /// What date is the last date that funding can be approved for, this is set as part of the output file generation.
    /// </summary>
    public DateOnly? LastFundingApprovalEndDate { get; private set; }

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
            Type = "Access to HE",
            Status = "Approved",
            StartDate = startDateForRegistration,
            LastDateForRegistration = lastDateForRegistration,
            SectorSubjectArea = sectorSubjectArea
        };
    }

    public RegulatedQaaQualification SetFundingApprovalEndDate(DateTime publicationDate)
    {
        //If the Last Day for Registration is after the next website publication date then the Funding Approval End Date should be between the Last Day for Registration and the End of the Academic Year
        // 
        // If the Last Day for Registration is before the next website publication date but after approval end date  on our current website then the Funding Approval End Date should be the Last Day for Registration
        // 
        // If the Last Day for Registration is before the current Funding Approval End Date on the website date then the Funding Approval End Date should be the same as before (i.e. no change to Funding Approval End Date)

        var publicationDateOnly = DateOnly.FromDateTime(publicationDate);
    
        if (LastDateForRegistration > publicationDateOnly)
        {
            var dates = new List<DateOnly>
            {
                LastDateForRegistration,
                GetAcademicYear()
            };

            LastFundingApprovalEndDate = dates.Min();
            return this;
        }

        if (LastDateForRegistration < publicationDateOnly)
        {
            if (LastDateForRegistration > LastFundingApprovalEndDate || 
                LastFundingApprovalEndDate is null)
            {
                LastFundingApprovalEndDate = LastDateForRegistration;
            }
        }

        return this;
    }

    private static DateOnly GetAcademicYear()
    {
        var today = DateTime.Today;

        if (today > new DateTime(today.Year, 7, 31))
        {
            return new DateOnly(today.Year + 1, 7, 31);
        }

        return DateOnly.FromDateTime(today);
    }
}