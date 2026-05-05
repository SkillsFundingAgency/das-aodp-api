using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Models.Files;
namespace SFA.DAS.AODP.Data.Repositories.Files
{
    public interface IFileRecordRepository
    {
        Task<List<FileRecord>> GetFilesAsync(
            FileCategory category,
            Guid? applicationId,
            Guid? messageId,
            Guid? questionId);

        Task<IReadOnlyList<FileRecord>> GetByApplicationIdAsync(Guid applicationId);
        Task DeleteAsync(Guid fileId);
        Task<FileRecord?> GetByIdAsync(Guid fileId);
        Task<FileRecord> AddAsync(FileRecord fileRecord);
    }
}
