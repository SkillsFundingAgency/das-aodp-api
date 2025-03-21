using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationDiscussionHistoryRepository : IQualificationDiscussionHistoryRepository
    {
        private readonly IApplicationDbContext _context;

        public QualificationDiscussionHistoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

       public async Task CreateAsync(QualificationDiscussionHistory qualificationDiscussionHistory)
        {
            qualificationDiscussionHistory.Id = Guid.NewGuid();
            _context.QualificationDiscussionHistory.Add(qualificationDiscussionHistory);
            await _context.SaveChangesAsync();
        }
    }
}
