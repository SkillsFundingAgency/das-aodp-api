﻿namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationReviewSharingStatusByIdQueryResponse
    {
        public Guid ApplicationId { get; set; }
        public bool SharedWithSkillsEngland { get; set; }
        public bool SharedWithOfqual { get; set; }
    }
}