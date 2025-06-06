﻿using SFA.DAS.AODP.Data.Entities.Qualification;
using System;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference);
    Task<List<ChangedQualification>> GetChangedQualificationsAsync();
    Task<ProcessStatus> UpdateQualificationStatus(string qualificationReference, Guid processStatusId, int? version);
    Task<List<ProcessStatus>> GetProcessingStatuses();

    Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport();

    Task<Entities.Qualification.Qualification> GetByIdAsync(string qualificationReference);
}
