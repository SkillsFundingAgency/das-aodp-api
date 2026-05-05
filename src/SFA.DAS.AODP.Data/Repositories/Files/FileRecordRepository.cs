using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Models.Files;

namespace SFA.DAS.AODP.Data.Repositories.Files;

public class FileRecordRepository : IFileRecordRepository
{
    private readonly IApplicationDbContext _context;

    public FileRecordRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FileRecord>> GetFilesAsync(
        FileCategory category,
        Guid? applicationId,
        Guid? messageId,
        Guid? questionId)
    {
        IQueryable<FileRecord> query = _context.FileRecords
            .Where(f => f.FileCategory == category);

        if (applicationId.HasValue)
            query = query.Where(f => f.ApplicationId == applicationId);

        if (messageId.HasValue)
            query = query.Where(f => f.MessageId == messageId);

        if (questionId.HasValue)
            query = query.Where(f => f.QuestionId == questionId);

        return await query
            .OrderBy(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FileRecord>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _context.FileRecords
            .Where(f => f.ApplicationId == applicationId)
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid fileId)
    {
        var file = await _context.FileRecords
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file != null)
        {
            _context.FileRecords.Remove(file);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<FileRecord?> GetByIdAsync(Guid fileId)
    {
        return await _context.FileRecords
            .SingleOrDefaultAsync(f => f.Id == fileId);
    }

    public async Task<FileRecord> AddAsync(FileRecord fileRecord)
    {
        fileRecord.Id = Guid.NewGuid();
        await _context.FileRecords.AddAsync(fileRecord);
        await _context.SaveChangesAsync();

        return fileRecord;
    }

}