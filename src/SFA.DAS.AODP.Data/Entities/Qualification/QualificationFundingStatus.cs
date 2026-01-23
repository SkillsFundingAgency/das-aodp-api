using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

public class QualificationFundingStatus
{
    public Guid QualificationVersionId { get; set; }
    public Guid QualificationId { get; set; }
    public string? Qan { get; set; }
    public string? QualificationName { get; set; }
    public string? Level { get; set; }
    public string? Type { get; set; }
    public string? Ssa { get; set; }

    public bool? SixteenToEighteen { get; set; }
    public bool? NineteenPlus { get; set; }
    public bool? PreSixteen { get; set; }
    public bool? EighteenPlus { get; set; }

    public DateTime? OperationalStartDate { get; set; }
    public DateTime? OperationalEndDate { get; set; }

    public bool? EligibleForFunding { get; set; }
    public bool FundedInEngland { get; set; }

    public Guid? FundedQualificationSnapshotId { get; set; }
    public string? FundedLevel { get; set; }
    public string? FundedQualificationType { get; set; }
    public string? SubCategory { get; set; }
    public string? SectorSubjectArea { get; set; }
    public Guid? FundedStatus { get; set; }
    public DateTime? DateOfOfqualDataSnapshot { get; set; }

    public string? AwardingOrganisationName { get; set; }
    public string? FundingStatus { get; set; }
}
