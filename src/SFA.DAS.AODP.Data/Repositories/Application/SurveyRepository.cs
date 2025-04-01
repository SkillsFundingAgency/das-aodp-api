using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly IApplicationDbContext _context;

        public SurveyRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Data.Entities.Application.Survey survey)
        {
            survey.Id = Guid.NewGuid();
            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();
        }
    }
}
