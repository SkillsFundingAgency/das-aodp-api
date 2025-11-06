using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Import;
using System.Data;

namespace SFA.DAS.AODP.Data.Repositories.Import;

public class DefundingListRepository : IDefundingListRepository
{
    private readonly IApplicationDbContext _context;

    public DefundingListRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task BulkInsertAsync(IEnumerable<DefundingList> items, CancellationToken cancellationToken = default)
    {
        if (items == null) return;

        _context.DefundingLists.AddRange(items);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteDuplicateDefundingListsAsync(string? qan = null, CancellationToken cancellationToken = default)
    {
        if (!(_context is ApplicationDbContext dbContext))
            throw new InvalidOperationException("Unable to execute stored procedure: unexpected DbContext type.");

        var conn = dbContext.Database.GetDbConnection();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "[dbo].[proc_DeleteDuplicateDefundingLists]";
        cmd.CommandType = CommandType.StoredProcedure;

        var param = cmd.CreateParameter();
        param.ParameterName = "@Qan";
        param.Value = (object?)qan ?? DBNull.Value;
        cmd.Parameters.Add(param);

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(cancellationToken);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var deleted = reader["DeletedRows"];
            if (deleted != DBNull.Value && int.TryParse(deleted.ToString(), out var deletedCount))
            {
                return deletedCount;
            }
        }

        return 0;
    }
}
