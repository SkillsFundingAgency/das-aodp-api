using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class QuestionRepository : IQuestionRepository
{
    private readonly IApplicationDbContext _context;

    public QuestionRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a question on a page, throws if no linked section is found, 
    /// or if the linked form version isn't in draft status. 
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    /// <exception cref="NoForeignKeyException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Question> Create(Question question)
    {
        if (!await _context.Pages.AnyAsync(v => v.Id == question.PageId))
            throw new NoForeignKeyException(question.PageId);

        if (!await _context.Pages.AnyAsync(v => v.Id == question.PageId && v.Section.FormVersion.Status == FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();

        question.Id = Guid.NewGuid();
        question.Key = Guid.NewGuid();

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    /// <summary>
    /// Gets max order for question for given page id.
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public int GetMaxOrderByPageId(Guid pageId)
    {
        var res = _context.Questions.Where(v => v.PageId == pageId).Max(s => (int?)s.Order) ?? 0;
        return res;
    }

    /// <summary>
    /// Gets a question with a given Id. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Question> GetQuestionByIdAsync(Guid id)
    {
        var res = await _context.Questions
            .Include(q => q.QuestionValidation)
            .Include(q => q.QuestionOptions)
            .FirstOrDefaultAsync(v => v.Id == id);
        return res is null ? throw new RecordNotFoundException(id) : res;
    }


    /// <summary>
    /// Updates a section's data, throws if no section for the given Id can be found, 
    /// or if the linked form version is not in draft. 
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Question> Update(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
        return question;
    }

    /// <summary>
    /// Archives a question, throws is no question with a given Id is found, 
    /// or the linked form version isn't in draft. 
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task Archive(Guid questionId)
    {
        await ValidateQuestionForChange(questionId);

        _context.Questions.Remove(await GetQuestionByIdAsync(questionId));
        await _context.SaveChangesAsync();
    }


    public async Task<Question> GetQuestionDetailForRoutingAsync(Guid questionId)
    {
        return await _context.Questions
                        .Include(q => q.Page)
                        .ThenInclude(q => q.Section)
                        .Include(q => q.QuestionOptions)
                        .Include(q => q.Routes)
                        .FirstOrDefaultAsync(q => q.Id == questionId) ?? throw new RecordNotFoundException(questionId);
    }

    public async Task ValidateQuestionForChange(Guid questionId)
    {
        if (!await _context.Questions.AnyAsync(v => v.Id == questionId))
            throw new RecordNotFoundException(questionId);

        if (!await _context.Questions.AnyAsync(v => v.Id == questionId && v.Page.Section.FormVersion.Status == FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();
    }


    public async Task<string?> GetQuestionTypeById(Guid questionid)
    {
        return await _context.Questions.Where(q => q.Id == questionid).Select(q => q.Type).FirstOrDefaultAsync() ?? throw new RecordNotFoundException(questionid);
    }

    /// <summary>
    /// Finds a question with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveQuestionOrderUp(Guid id)
    {
        var modelToUpdate = await _context.Questions.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextHigherModel = await _context.Questions
            .OrderBy(v => v.Order)
            .Where(v => v.Order < modelToUpdate.Order)
            .FirstOrDefaultAsync();
        if (nextHigherModel is null)
            return true;
        var nextHighest = nextHigherModel.Order;
        nextHigherModel.Order = modelToUpdate.Order;
        modelToUpdate.Order = nextHighest;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Finds a question with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveQuestionOrderDown(Guid id)
    {
        var modelToUpdate = await _context.Questions.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextLowerModel = await _context.Questions
            .OrderByDescending(v => v.Order)
            .Where(v => v.Order > modelToUpdate.Order)
            .FirstOrDefaultAsync();
        if (nextLowerModel is null)
            return true;
        var nextLowest = nextLowerModel.Order;
        nextLowerModel.Order = modelToUpdate.Order;
        modelToUpdate.Order = nextLowest;
        await _context.SaveChangesAsync();

        return true;
    }
}
