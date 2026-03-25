using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Qualifications;
using QualificationDiscussionHistory = SFA.DAS.AODP.Data.Entities.Qualification.QualificationDiscussionHistory;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using static SFA.DAS.AODP.Data.Repositories.Qualification.IQualificationsRepository;
using ChangedQualification = Entities.Qualification.ChangedQualification;

public class QualificationsRepository(ApplicationDbContext context) : IQualificationsRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<ChangedQualification>> GetChangedQualificationsAsync()
    {
        return await _context.ChangedQualifications
            .ToListAsync();
    }
    public async Task AddQualificationDiscussionHistory(QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference)
    {
        var qual = await _context.Qualification.FirstOrDefaultAsync(v => v.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        qualificationDiscussionHistory.QualificationId = qual.Id;
        qualificationDiscussionHistory.Timestamp = DateTime.UtcNow;
        qualificationDiscussionHistory.ActionTypeId = await GetActionTypeId(qual.Id);
        _context.QualificationDiscussionHistory.Add(qualificationDiscussionHistory);
        await _context.SaveChangesAsync();
    }

    private async Task<Guid> GetActionTypeId(Guid qualificationId)
    {
        var lastItem = await _context.QualificationDiscussionHistory
            .OrderByDescending(v => v.Timestamp)
            .FirstOrDefaultAsync(v => v.QualificationId == qualificationId);
        if (lastItem is not null)
        {
            return lastItem.ActionTypeId;
        }
        return await _context.ActionType.Where(v => v.Description == "No Action Required")
            .Select(v => v.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<ProcessStatus> UpdateQualificationStatus(string qualificationReference, Guid processStatusId, int? version)
    {
        var query = _context.QualificationVersions
            .Include(v => v.Qualification)
            .OrderByDescending(v => v.Version);
        var qual = version is not null ?
            await query.FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference && v.Version == version) :
            await query.FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        var processStatus = await GetProcessStatusOrThrow(processStatusId);
        qual.ProcessStatusId = processStatusId;
        await _context.SaveChangesAsync();
        return processStatus;
    }

    public async Task<QualificationVersions?> GetQualificationVersionByQanAsync(string qualificationReference, int? version, CancellationToken cancellationToken) 
        => await _context.QualificationVersions
            .Include(o => o.ProcessStatus)
            .Include(o => o.LifecycleStage)
            .Include(o => o.Qualification)
            .Where(x => x.Qualification.Qan == qualificationReference)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<ProcessStatus>> GetProcessingStatuses() => await _context.ProcessStatus.ToListAsync();
    public async Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport()
    {
        return await _context.ChangedQualificationExport.ToListAsync<ChangedQualificationExport>();
    }

    public async Task<Entities.Qualification.Qualification> GetByIdAsync(string qualificationReference)
    {
        return await _context
                    .Qualification
                    .Include(a => a.QualificationVersions)
                    .Where(v => v.Qan == qualificationReference)
                    .FirstOrDefaultAsync();

    }

    public async Task<ProcessStatus> GetProcessStatusOrThrow(Guid processStatusId)
    {
        var processStatus = await _context.ProcessStatus
            .FirstOrDefaultAsync(v => v.Id == processStatusId);

        if (processStatus is null)
        {
            throw new NoForeignKeyException(processStatusId);
        }

        return processStatus;
    }

    public async Task<BulkUpdateQualificationStatusWithHistoryResult> BulkUpdateQualificationStatusWithHistoryAsync(
    IReadOnlyCollection<Guid> qualificationIds,
    Guid processStatusId,
    string userDisplayName,
    string? comment,
    CancellationToken cancellationToken = default)
    {
        var status = await GetProcessStatusOrThrow(processStatusId);

        var ids = qualificationIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return CreateEmptyResult(status);

        var latest = await _context.QualificationVersions
            .AsNoTracking()
            .Where(v => ids.Contains(v.QualificationId))
            .GroupBy(v => v.QualificationId)
            .Select(g => g
                .OrderByDescending(x => x.Version)
                .Select(x => new
                {
                    QualificationId = x.QualificationId,
                    QualificationVersionId = x.Id,
                    QualificationReference = x.Qualification.Qan
                })
                .First())
            .ToListAsync(cancellationToken);

        var foundQualificationIds = latest.Select(x => x.QualificationId).ToHashSet();
        var missingQualificationIds = ids.Where(id => !foundQualificationIds.Contains(id)).ToList();

        if (latest.Count == 0)
            return CreateEmptyResult(status, missingQualificationIds);

        var defaultActionTypeId = await _context.ActionType
            .AsNoTracking()
            .Where(a => a.Description == "No Action Required")
            .Select(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var lastActionTypeByQualification = await _context.QualificationDiscussionHistory
            .AsNoTracking()
            .Where(h => foundQualificationIds.Contains(h.QualificationId))
            .GroupBy(h => h.QualificationId)
            .Select(g => new
            {
                QualificationId = g.Key,
                ActionTypeId = g
                    .OrderByDescending(x => x.Timestamp)
                    .Select(x => x.ActionTypeId)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.QualificationId, x => x.ActionTypeId, cancellationToken);

        var now = DateTime.UtcNow;
        var historyTitle = $"Updated status to: {status.Name}";

        var succeeded = new List<(Guid QualificationVersionId, string Qan)>(latest.Count);
        var statusUpdateFailed = new List<(Guid QualificationId, string Qan, string Message)>();
        var historyFailed = new List<(Guid QualificationId, string Qan, string Message)>();

        for (var i = 0; i < latest.Count; i++)
        {
            var item = latest[i];

            try
            {
                var rows = await _context.QualificationVersions
                    .Where(v => v.Id == item.QualificationVersionId)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(v => v.ProcessStatusId, _ => status.Id),
                        cancellationToken);

                if (rows == 0)
                    throw new InvalidOperationException($"No rows updated for versionId={item.QualificationVersionId}");
            }
            catch
            {
                _context.ChangeTracker.Clear();
                statusUpdateFailed.Add((item.QualificationId, item.QualificationReference, "Failed to update status"));
                continue; //Don't add history if status update fails for a qualification
            }

            try
            {
                var actionTypeId =
                    lastActionTypeByQualification.TryGetValue(item.QualificationId, out var lastId) && lastId != Guid.Empty
                        ? lastId
                        : defaultActionTypeId;

                var history = new QualificationDiscussionHistory
                {
                    QualificationId = item.QualificationId,
                    Timestamp = now,
                    UserDisplayName = userDisplayName,
                    Notes = comment,
                    Title = historyTitle,
                    ActionTypeId = actionTypeId
                };

                _context.QualificationDiscussionHistory.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                succeeded.Add((item.QualificationVersionId, item.QualificationReference));
            }
            catch
            {
                _context.ChangeTracker.Clear();
                historyFailed.Add((item.QualificationId, item.QualificationReference, "Status updated but failed to add history"));
            }
        }

        return new BulkUpdateQualificationStatusWithHistoryResult
        {
            Status = status,
            Succeeded = succeeded,
            MissingIds = missingQualificationIds,
            StatusUpdateFailed = statusUpdateFailed,
            HistoryFailed = historyFailed
        };
    }

    public async Task<List<SearchedQualification>> GetSearchedQualificationByNameAsync(string searchTerm)
    {
        var pattern = $"%{searchTerm.Trim()}%";

        return await _context.QualificationFundingStatus
            .AsNoTracking()
            .Where(q => EF.Functions.Like((q.QualificationName ?? string.Empty), pattern))
            .OrderBy(q => q.FundingStatus)
            .ThenBy(q => q.AwardingOrganisationName)
            .ThenBy(q => q.QualificationName)
            .Select(q => new SearchedQualification
            {
                Id = q.QualificationId,
                QualificationName = q.QualificationName,
                Qan = q.Qan!,
                Status = q.FundedStatus
            })
            .ToListAsync();
    }

    public async Task<List<QualificationVersions>> GetQualificationVersionsByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.Distinct().ToList();

        return await _context.QualificationVersions
            .Include(v => v.Qualification)
            .Where(v => idList.Contains(v.Id))
            .ToListAsync();
    }

    private static BulkUpdateQualificationStatusWithHistoryResult CreateEmptyResult(
        ProcessStatus status,
        IReadOnlyCollection<Guid>? missingIds = null)
    {
        return new BulkUpdateQualificationStatusWithHistoryResult
        {
            Status = status,
            Succeeded = Array.Empty<(Guid, string)>(),
            MissingIds = (IReadOnlyList<Guid>)(missingIds ?? Array.Empty<Guid>()),
            StatusUpdateFailed = Array.Empty<(Guid, string, string)>(),
            HistoryFailed = Array.Empty<(Guid, string, string)>()
        };
    }
}
