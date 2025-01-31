using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories;

public class QuestionValidationRepository : IQuestionValidationRepository
{
    private readonly IApplicationDbContext _context;

    public QuestionValidationRepository(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<QuestionValidation?> GetQuestionValidationsByQuestionIdAsync(Guid questionId)
    {
        return await _context.QuestionValidations.FirstOrDefaultAsync(q => q.QuestionId == questionId);
    }

    public async Task UpsertAsync(QuestionValidation validation)
    {
        if (validation.Id == default)
        {
            validation.Id = Guid.NewGuid();
            _context.QuestionValidations.Add(validation);
        }
        else
        {
            _context.QuestionValidations.Update(validation);
        }
        await _context.SaveChangesAsync();
    }
}
