
namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IFormRepository
    {
        Task Archive(Guid formId);
        int GetMaxOrder();
        Task<bool> MoveFormOrderDown(Guid id);
        Task<bool> MoveFormOrderUp(Guid id);
    }
}