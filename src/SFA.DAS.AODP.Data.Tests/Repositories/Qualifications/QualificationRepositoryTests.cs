using AutoFixture;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Qualifications;

public class QualificationRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly QualificationsRepository _repository;
    private readonly Fixture _fixture;

    private readonly Guid _lifeCycleStageNew = new("00000000-0000-0000-0000-000000000001");
    private readonly Guid _processStageNoAction = new("00000000-0000-0000-0000-000000000001");

    public QualificationRepositoryTests()
    {
        _fixture = new Fixture();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ApplicationDbContext" + Guid.NewGuid()).Options;
        _dbContext = new ApplicationDbContext(options);
        _repository = new QualificationsRepository(_dbContext);
    }

    [Fact]
    public async Task GetQualificationVersionByQanAsync_QualificationDoesNotExist_ReturnNull()
    {
        // Arrange
        // Negative data
        await CreateQualificationRecordSetAsync(12, "qan1", "qual1", CancellationToken.None);

        // Act
        var result = await _repository.GetQualificationVersionByQanAsync("qan2", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetQualificationVersionByQanAsync_QualificationDoesExist_ReturnQualification()
    {
        // Arrange
        await CreateQualificationRecordSetAsync(12, "qan1", "qual1", CancellationToken.None);

        // Negative data
        await CreateQualificationRecordSetAsync(12, "qan2", "qual2", CancellationToken.None);

        // Act
        var result = await _repository.GetQualificationVersionByQanAsync("qan1", CancellationToken.None);

        // Assert
        var loaded = await _dbContext.QualificationVersions.Include(o => o.Qualification)
            .SingleAsync(o => o.Qualification.Qan == "qan1");

        Assert.Equal("qan1", loaded.Qualification.Qan);
    }

    private async Task CreateQualificationRecordSetAsync(int organisationId, string qualificationNumber, string qualificationName, CancellationToken cancellationToken)
    {
        var orgId = Guid.NewGuid();
        var qan1_organisation = _fixture.Build<AwardingOrganisation>()
            .Without(w => w.Qualifications)
            .Without(w => w.QualificationVersions)
            .With(w => w.Ukprn, organisationId)
            .With(w => w.Id, orgId)
            .Create();

        var qan1_qualification = _fixture.Build<Qualification>()
            .Without(w => w.Qualifications)
            .Without(w => w.QualificationDiscussionHistories)
            .Without(w => w.QualificationVersions)
            .With(w => w.QualificationName, qualificationName)
            .With(w => w.Qan, qualificationNumber)
            .Create();

        var qan1_qualificationVersionFieldChange1 = _fixture.Build<VersionFieldChange>()
            .Without(w => w.QualificationVersions)
            .With(w => w.QualificationVersionNumber, 1)
            .With(w => w.ChangedFieldNames, "Glh, Status")
            .Create();

        var qan1_qualificationVersion1 = _fixture.Build<QualificationVersions>()
            .Without(w => w.Qualification)
            .Without(w => w.Organisation)
            .Without(w => w.LifecycleStage)
            .Without(w => w.ProcessStatus)
            .With(w => w.VersionFieldChanges, qan1_qualificationVersionFieldChange1)
            .With(w => w.Version, 1)
            .With(w => w.QualificationId, qan1_qualification.Id)
            .With(w => w.AwardingOrganisationId, qan1_organisation.Id)
            .With(w => w.OfferedInEngland, true)
            .With(w => w.Glh, 5)
            .With(w => w.Tqt, 10)
            .With(w => w.OperationalStartDate, Enum.QualificationReference.MinOperationalDate)
            .With(w => w.LifecycleStageId, _lifeCycleStageNew)
            .With(w => w.ProcessStatusId, _processStageNoAction)
            .Create();

        var organisations = new List<AwardingOrganisation>() { qan1_organisation };
        var qualifications = new List<Qualification>() { qan1_qualification };
        var qualificationVersions = new List<QualificationVersions>() { qan1_qualificationVersion1 };
        var qualificationVersionFieldChanges = new List<VersionFieldChange>() { qan1_qualificationVersionFieldChange1 };

        await _dbContext.AddRangeAsync(organisations, cancellationToken);
        await _dbContext.AddRangeAsync(qualifications, cancellationToken);
        await _dbContext.AddRangeAsync(qualificationVersions, cancellationToken);
        await _dbContext.AddRangeAsync(qualificationVersionFieldChanges, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

}