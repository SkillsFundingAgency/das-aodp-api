using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
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

        public async Task<Dictionary<Guid, Guid>> CopyQuestionOptionsForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds)
        {
            var oldNewIds = new Dictionary<Guid, Guid>();
            var oldIds = oldNewQuestionIds.Keys.ToList();

            var toMigrate = await _context.QuestionOptions.AsNoTracking().Where(v => oldIds.Contains(v.QuestionId)).ToListAsync();
            foreach (var entity in toMigrate)
            {
                var oldId = entity.Id;
                entity.QuestionId = oldNewQuestionIds[entity.QuestionId];
                entity.Id = Guid.NewGuid();

                oldNewIds.Add(oldId, entity.Id);
            }
            await _context.QuestionOptions.AddRangeAsync(toMigrate);
            await _context.SaveChangesAsync();

            return oldNewIds;
        }
    }
}