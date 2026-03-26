using AutoFixture;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Data.Tests
{
    public class QualificationsRepositoryBulkUpdateTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _dbContext;
        private readonly QualificationsRepository _repository;
        private readonly Fixture _fixture;

        private static readonly Guid ProcessStatusDecision = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private static readonly Guid ActionTypeNoAction = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private static readonly Guid ActionTypeDecision = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private static readonly Guid LifecycleStageChanged = Guid.Parse("00000000-0000-0000-0000-000000000010");
        private static readonly Guid VersionFieldChangesSeed = Guid.Parse("00000000-0000-0000-0000-000000000020");

        private const string UserDisplayName = "Test User";
        private const string Comment = "Test comment";
        private const string SeedTitle = "seed";
        private const string SeedNotes = "seed";

        public QualificationsRepositoryBulkUpdateTests()
        {
            _fixture = new Fixture();

            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();

            SeedReferenceData();

            _repository = new QualificationsRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _connection.Dispose();

            GC.SuppressFinalize(this);
        }

        private void SeedReferenceData()
        {
            _dbContext.ActionType.AddRange(
                new ActionType { Id = ActionTypeNoAction, Description = "No Action Required" },
                new ActionType { Id = ActionTypeDecision, Description = "Action Required" });

            _dbContext.ProcessStatus.Add(
                new ProcessStatus { Id = ProcessStatusDecision, Name = Enum.ProcessStatus.DecisionRequired });

            _dbContext.LifecycleStages.Add(
                new LifecycleStage { Id = LifecycleStageChanged, Name = Enum.LifeCycleStage.Changed });

            _dbContext.VersionFieldChanges.Add(
                new VersionFieldChange { Id = VersionFieldChangesSeed, QualificationVersionNumber = 1, ChangedFieldNames = "Seed" });

            _dbContext.SaveChanges();
        }

        private (Guid QualificationId, string Qan) CreateQualificationWithVersion(Guid awardingOrgId)
        {
            var qualificationId = Guid.NewGuid();
            var qan = $"QAN-{Guid.NewGuid():N}".Substring(0, 10);

            var qualification = new Qualification
            {
                Id = qualificationId,
                Qan = qan,
                QualificationName = "Test Qualification"
            };

            var awardingOrg = new AwardingOrganisation
            {
                Id = awardingOrgId,
                Ukprn = 12345678
            };

            var now = DateTime.UtcNow;

            var version = new QualificationVersions
            {
                Id = Guid.NewGuid(),
                QualificationId = qualificationId,
                AwardingOrganisationId = awardingOrgId,
                ProcessStatusId = ProcessStatusDecision,
                VersionFieldChangesId = VersionFieldChangesSeed,
                LifecycleStageId = LifecycleStageChanged,
                AdditionalKeyChangesReceivedFlag = 0,
                Status = "Available to learners",
                Type = "Basic Skills",
                Ssa = "Marketing and sales",
                Level = "Level 2",
                SubLevel = "None",
                EqfLevel = "None",
                RegulationStartDate = now,
                OperationalStartDate = Enum.QualificationReference.MinOperationalDate,
                OfferedInEngland = true,
                OfferedInNi = false,
                RegulatedByNorthernIreland = false,
                LastUpdatedDate = now,
                UiLastUpdatedDate = now,
                InsertedDate = now,
                Version = 1,
                Glh = 5,
                Tqt = 10
            };

            _dbContext.AddRange(qualification, awardingOrg, version);
            _dbContext.SaveChanges();

            return (qualificationId, qan);
        }

        private Guid GetLatestVersionId(Guid qualificationId)
        {
            return _dbContext.QualificationVersions
                .Where(v => v.QualificationId == qualificationId)
                .OrderByDescending(v => v.Version)
                .Select(v => v.Id)
                .First();
        }

        [Fact]
        public async Task BulkUpdateQualificationStatusWithHistoryAsync_ThrowsNoForeignKeyException_WhenProcessStatusMissing()
        {
            var missingStatusId = Guid.NewGuid();

            var act = () => _repository.BulkUpdateQualificationStatusWithHistoryAsync(
                new[] { Guid.NewGuid() },
                missingStatusId,
                UserDisplayName,
                Comment,
                CancellationToken.None);

            await Assert.ThrowsAsync<NoForeignKeyException>(act);
        }

        [Fact]
        public async Task BulkUpdateQualificationStatusWithHistoryAsync_ReturnsEmptyResult_WhenNoValidIds()
        {
            var result = await _repository.BulkUpdateQualificationStatusWithHistoryAsync(
                new[] { Guid.Empty, Guid.Empty },
                ProcessStatusDecision,
                UserDisplayName,
                Comment,
                CancellationToken.None);

            Assert.Equal(ProcessStatusDecision, result.Status.Id);
            Assert.Empty(result.Succeeded);
            Assert.Empty(result.MissingIds);
            Assert.Empty(result.StatusUpdateFailed);
            Assert.Empty(result.HistoryFailed);
        }

        [Fact]
        public async Task BulkUpdateQualificationStatusWithHistoryAsync_UpdatesLatestVersionAndCreatesHistory()
        {
            var awardingOrgId = Guid.NewGuid();
            var (qualificationId, qan) = CreateQualificationWithVersion(awardingOrgId);

            var latestVersionId = GetLatestVersionId(qualificationId);

            var result = await _repository.BulkUpdateQualificationStatusWithHistoryAsync(
                new[] { qualificationId },
                ProcessStatusDecision,
                UserDisplayName,
                Comment,
                CancellationToken.None);

            Assert.Single(result.Succeeded);
            Assert.Empty(result.MissingIds);
            Assert.Empty(result.StatusUpdateFailed);
            Assert.Empty(result.HistoryFailed);

            var succeeded = result.Succeeded.Single();
            Assert.Equal(latestVersionId, succeeded.QualificationVersionId);
            Assert.Equal(qan, succeeded.Qan);

            var updatedStatusId = await _dbContext.QualificationVersions
                .Where(v => v.Id == latestVersionId)
                .Select(v => v.ProcessStatusId)
                .SingleAsync();

            Assert.Equal(ProcessStatusDecision, updatedStatusId);

            var history = await _dbContext.QualificationDiscussionHistory
                .Where(h => h.QualificationId == qualificationId)
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefaultAsync();

            Assert.NotNull(history);
            Assert.Equal(UserDisplayName, history!.UserDisplayName);
            Assert.Equal(Comment, history.Notes);
            Assert.Contains(result.Status.Name.ToString(), history.Title);
        }

        [Fact]
        public async Task BulkUpdateQualificationStatusWithHistoryAsync_StatusSuccess_HistorySuccess()
        {
            var awardingOrgId = Guid.NewGuid();
            var (qualificationId, _) = CreateQualificationWithVersion(awardingOrgId);

            var result = await _repository.BulkUpdateQualificationStatusWithHistoryAsync(
                new[] { qualificationId },
                ProcessStatusDecision,
                "user",
                "comment",
                CancellationToken.None);

            Assert.Single(result.Succeeded);
            Assert.Empty(result.StatusUpdateFailed);
            Assert.Empty(result.HistoryFailed);
            Assert.Empty(result.MissingIds);
        }

        [Fact]
        public async Task BulkUpdateQualificationStatusWithHistoryAsync_StatusSuccess_HistoryFails()
        {
            var awardingOrgId = Guid.NewGuid();
            var (qualificationId, _) = CreateQualificationWithVersion(awardingOrgId);

            _dbContext.ActionType.RemoveRange(_dbContext.ActionType);
            _dbContext.SaveChanges();

            var result = await _repository.BulkUpdateQualificationStatusWithHistoryAsync(
                new[] { qualificationId },
                ProcessStatusDecision,
                "user",
                "comment",
                CancellationToken.None);

            Assert.Empty(result.Succeeded);
            Assert.Empty(result.StatusUpdateFailed);
            Assert.Single(result.HistoryFailed);
            Assert.Empty(result.MissingIds);

            Assert.Equal(qualificationId, result.HistoryFailed.Single().QualificationId);
        }

        
        // TODO: Add a test for "status update fails" when using SQL Server.
        // SQLite cannot simulate the real-world scenario where ExecuteUpdateAsync
        // returns 0 rows updated without throwing (e.g., due to race conditions,
        // triggers, or lock timeouts). This test must be implemented using either
        // SQL Server LocalDB or a mocked abstraction around ExecuteUpdateAsync.



    }
}
