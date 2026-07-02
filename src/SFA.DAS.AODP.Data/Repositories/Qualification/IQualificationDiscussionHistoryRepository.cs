using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationDiscussionHistoryRepository
    {
        Task CreateAsync(QualificationDiscussionHistory qualificationDiscussionHistory);

        void AddDiscussionHistories(List<QualificationDiscussionHistory> histories);
    }
}