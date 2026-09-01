using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification
{
    public class QaaQualificationDiscussionHistoryRepository : IQaaQualificationDiscussionHistoryRepository
    {
        private readonly IApplicationDbContext _context;

        public QaaQualificationDiscussionHistoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(QaaQualificationDiscussionHistory qaaQualificationDiscussionHistory)
        {
            qaaQualificationDiscussionHistory.Id = Guid.NewGuid();
            _context.QaaQualificationDiscussionHistory.Add(qaaQualificationDiscussionHistory);
            await _context.SaveChangesAsync();
        }

        public void AddDiscussionHistories(List<QaaQualificationDiscussionHistory> histories)
        {
            _context.QaaQualificationDiscussionHistory.AddRange(histories);
        }
    }
}
