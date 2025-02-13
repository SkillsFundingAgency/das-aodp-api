using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationQuestionAnswerRepository : IApplicationQuestionAnswerRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationQuestionAnswerRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationQuestionAnswer> Create(ApplicationQuestionAnswer questionAnswer)
        {
            questionAnswer.Id = Guid.NewGuid();

            await _context
            .ApplicationQuestionAnswers
            .AddAsync(questionAnswer);
            await _context.SaveChangesAsync();

            return questionAnswer;
        }

        public async Task<List<ApplicationQuestionAnswer>> GetAnswersByApplicationAndPageId(Guid applicationId, Guid pageId)
        {
            return await _context
            .ApplicationQuestionAnswers
            .Where(
                a => a.ApplicationPage.PageId == pageId &&
                a.ApplicationPage.ApplicationId == applicationId
            ).ToListAsync();
        }

        public async Task<ApplicationQuestionAnswer> Update(ApplicationQuestionAnswer questionAnswer)
        {
            _context
            .ApplicationQuestionAnswers
            .Update(questionAnswer);
            await _context.SaveChangesAsync();

            return questionAnswer;
        }

        public async Task UpsertAsync(List<ApplicationQuestionAnswer> questionAnswers)
        {
            foreach (var answer in questionAnswers)
            {
                if (answer.Id == default)
                {
                    answer.Id = Guid.NewGuid();

                    _context.ApplicationQuestionAnswers.Add(answer);
                }
                else
                {
                    _context.ApplicationQuestionAnswers.Update(answer);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
