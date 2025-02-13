﻿using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetNewQualificationsQueryResponse
    {
        public bool Success { get; set; }
        public List<NewQualification> NewQualifications { get; set; } = new();
    }
}
