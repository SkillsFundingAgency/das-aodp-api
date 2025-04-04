using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Feedback;

namespace SFA.DAS.AODP.Data.Repositories.Feedback
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly IApplicationDbContext _context;

        public SurveyRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Survey survey)
        {
            survey.Id = Guid.NewGuid();
            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();
        }
    }
}
