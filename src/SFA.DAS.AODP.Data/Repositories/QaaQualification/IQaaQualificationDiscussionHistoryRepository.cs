using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification
{
    public interface IQaaQualificationDiscussionHistoryRepository
    {
        Task CreateAsync(QaaQualificationDiscussionHistory qaaQualificationDiscussionHistory);

        void AddDiscussionHistories(List<QaaQualificationDiscussionHistory> histories);
    }
}
