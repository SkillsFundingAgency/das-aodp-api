using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class FormRepository : IFormRepository
{
    private readonly IApplicationDbContext _context;

    public FormRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public int GetMaxOrder()
    {
        var res = _context.Forms.Max(s => (int?)s.Order) ?? 0;
        return res;
    }

    /// <summary>
    /// Finds a form version with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveFormOrderUp(Guid id)
    {
        var modelToUpdate = await _context.Forms.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextHigherModel = await _context.Forms
            .OrderByDescending(v => v.Order)
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
    /// Finds a form version with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveFormOrderDown(Guid id)
    {
        var modelToUpdate = await _context.Forms.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextLowerModel = await _context.Forms
            .OrderBy(v => v.Order)
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

    /// <summary>
    /// Sets the status of a form with a given Id to the status of archived. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task Archive(Guid formId)
    {
        var form = await _context.Forms
        .FirstOrDefaultAsync(v => v.Id == formId) ?? throw new RecordNotFoundException(formId);
        form.Status = FormStatus.Deleted.ToString();
        await _context.SaveChangesAsync();
    }
}
