﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Models.Qualifications
{
    public class QualificationDetails
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public string? Status { get; set; }
        public Guid? StatusId { get; set; }
        public string? Priority { get; set; }
        public string? Changes { get; set; }
        public string? QualificationReference { get; set; }
        public string? AwardingOrganisation { get; set; }
        public string? Title { get; set; }
        public string? QualificationType { get; set; }
        public string? Level { get; set; }
        public string? ProposedChanges { get; set; }
        public string? AgeGroup { get; set; }
        public string? Category { get; set; }
        public string? Subject { get; set; }
        public string? SectorSubjectArea { get; set; }
        public string? Comments { get; set; }
        public List<QualificationDiscussionHistory> QualificationDiscussionHistories { get; set; }
    }
}
