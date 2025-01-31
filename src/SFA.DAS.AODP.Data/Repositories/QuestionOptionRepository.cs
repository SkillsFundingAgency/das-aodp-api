using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories
{
    public class QuestionOptionRepository : IQuestionOptionRepository
    {
        private readonly IApplicationDbContext _context;

        public QuestionOptionRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RemoveAsync(List<QuestionOption> optionsToRemove)
        {
            _context.QuestionOptions.RemoveRange(optionsToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task UpsertAsync(List<QuestionOption> options)
        {
            foreach (var option in options)
            {
                if (option.Id == default)
                {
                    option.Id = Guid.NewGuid();
                    _context.QuestionOptions.Add(option);

                }
                else
                {
                    _context.QuestionOptions.Update(option);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}