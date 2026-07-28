using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Models.Qualifications
{
    public class NewQualification
    {
        public Guid QualificationId { get; set; }
        public string? Title { get; set; }
        public string? Reference { get; set; }
        public string? AwardingOrganisation { get; set; }
        public string? Status { get; set; }
        public string? AgeGroup { get; set; }
        public bool? EligibleForFunding { get; set; }
    }
}
