// using Microsoft.EntityFrameworkCore;
// using SFA.DAS.AODP.Data.Context;
// using SFA.DAS.AODP.Data.Entities.FormBuilder;
// using SFA.DAS.AODP.Models.Form;

// namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;

// public class QuestionValidationRepository : IQuestionValidationRepository
// {
//     private readonly IApplicationDbContext _context;

//     public QuestionValidationRepository(IApplicationDbContext context)
//     {
//         _context = context;
//     }
//     public async Task<QuestionValidation?> GetQuestionValidationsByQuestionIdAsync(Guid questionId)
//     {
//         return await _context.QuestionValidations.FirstOrDefaultAsync(q => q.QuestionId == questionId);
//     }

//     public async Task UpsertAsync(QuestionValidation validation)
//     {
//         if (validation.Id == default)
//         {
//             validation.Id = Guid.NewGuid();
//             _context.QuestionValidations.Add(validation);
//         }
//         else
//         {
//             _context.QuestionValidations.Update(validation);
//         }
//         await _context.SaveChangesAsync();
//     }

//     public async Task CopyQuestionValidationForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds)
//     {
//         var oldIds = oldNewQuestionIds.Keys.ToList();

//         var toMigrate = await _context.QuestionValidations.AsNoTracking().Where(v => oldIds.Contains(v.QuestionId)).ToListAsync();
//         foreach (var entity in toMigrate)
//         {
//             entity.QuestionId = oldNewQuestionIds[entity.QuestionId];
//             entity.Id = Guid.NewGuid();
//         }
//         await _context.QuestionValidations.AddRangeAsync(toMigrate);
//         await _context.SaveChangesAsync();
//     }

// }
