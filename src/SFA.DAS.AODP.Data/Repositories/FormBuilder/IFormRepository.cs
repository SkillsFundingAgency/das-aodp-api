
namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IFormRepository
    {
        int GetMaxOrder();
        Task<bool> MoveFormOrderDown(Guid id);
        Task<bool> MoveFormVersionOrderUp(Guid id);
    }
}