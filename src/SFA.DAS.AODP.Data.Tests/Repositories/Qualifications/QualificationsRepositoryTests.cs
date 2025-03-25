using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Data.Tests
{
    public class QualificationsRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private NewQualificationsRepository _repository;       
        private Fixture _fixture;
        private Guid LifeCycleStageNew = new Guid("00000000-0000-0000-0000-000000000001");
        private Guid LifeCycleStageChanged = new Guid("00000000-0000-0000-0000-000000000002");
        private Guid ProcessStageNoAction = new Guid("00000000-0000-0000-0000-000000000001");
        private Guid ProcessStageDecision = new Guid("00000000-0000-0000-0000-000000000002");
        private Guid ActionTypeNoAction = new Guid("00000000-0000-0000-0000-000000000001");
        private Guid ActionTypeDecision = new Guid("00000000-0000-0000-0000-000000000002");

        public QualificationsRepositoryTests()
        {
            _fixture = new Fixture();          
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ApplicationDbContext" + Guid.NewGuid()).Options;
            var configuration = new Mock<IConfiguration>();
            _dbContext = new ApplicationDbContext(options, configuration.Object);
            _repository = new NewQualificationsRepository(_dbContext);
        }

        //[Fact]
        //public async Task GetAllNewQualificationsAsync_then_NewQualificationsData_Is_Returned()
        //{
        //    var qualificationNumber1 = "1000001";
        //    var qualificationName1 = "TestQualName1";
        //    var qualificationNumber2 = "1000002";
        //    var qualificationName2 = "TestQualName2";

        //    // Arrange
        //    var mockRepository = new Mock<IApplicationDbContext>();
        //    var filter = new NewQualificationsFilter();
        //    var record1 = CreateNewQualificationsViewData(qualificationNumber1, qualificationName1);
        //    var record2 = CreateNewQualificationsViewData(qualificationNumber2, qualificationName2);
        //    var sourceList = new List<Entities.Qualification.QualificationNewReviewRequired>() { record1, record2 };
        //    var queryable = sourceList.AsAsyncQueryable();
        //    var mockDbSet = new Mock<DbSet<Entities.Qualification.QualificationNewReviewRequired>>();
        //    var mockDbSetAsQueryable = mockDbSet.As<Queryable<Entities.Qualification.QualificationNewReviewRequired>>();
        //    mockDbSetAsQueryable.Setup(m => m.Provider).Returns(queryable.Provider);
        //    mockDbSetAsQueryable.Setup(m => m.Expression).Returns(queryable.Expression);
        //    mockDbSetAsQueryable.Setup(m => m.ElementType).Returns(queryable.ElementType);
        //    mockDbSetAsQueryable.Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());            
        //    mockRepository.SetupGet(s => s.QualificationNewReviewRequired).ReturnsDbSet(sourceList, mockDbSet);

        //    _repository = new NewQualificationsRepository(mockRepository.Object);

        //    // Act
        //    var result = await _repository.GetAllNewQualificationsAsync(0, 0, filter);

        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.TotalRecords);
        //    Assert.Equal(2, result.Data.Count);
        //    Assert.Equal(qualificationName1, result.Data[0].Title);
        //    Assert.Equal(qualificationName2, result.Data[1].Title);
        //}


        private async Task PopulateDbWithReferenceData()
        {
            var actionType1 = new ActionType() { Description = "No Action Required", Id = ActionTypeNoAction };
            var actionType2 = new ActionType() { Description = "Action Required", Id = ActionTypeDecision };
            var actionType3 = new ActionType() { Description = "Ignore", Id = Guid.NewGuid() };
            await _dbContext.AddRangeAsync(new List<ActionType>() { actionType1, actionType2, actionType3 });
            await _dbContext.SaveChangesAsync();

            var processStatus1 = new ProcessStatus() { Name = Enum.ProcessStatus.DecisionRequired, Id = ProcessStageDecision };
            var processStatus2 = new ProcessStatus() { Name = Enum.ProcessStatus.NoActionRequired, Id = ProcessStageNoAction };
            var processStatus3 = new ProcessStatus() { Name = Enum.ProcessStatus.Hold, Id = Guid.NewGuid() };
            var processStatus4 = new ProcessStatus() { Name = Enum.ProcessStatus.Rejected, Id = Guid.NewGuid() };
            var processStatus5 = new ProcessStatus() { Name = Enum.ProcessStatus.Approved, Id = Guid.NewGuid() };
            await _dbContext.AddRangeAsync(new List<ProcessStatus>() { processStatus1, processStatus2, processStatus3, processStatus4, processStatus5 });
            await _dbContext.SaveChangesAsync();

            var lifecycle1 = new LifecycleStage() { Name = Enum.LifeCycleStage.New, Id = LifeCycleStageNew };
            var lifecycle2 = new LifecycleStage() { Name = Enum.LifeCycleStage.Changed, Id = LifeCycleStageChanged };
            await _dbContext.AddRangeAsync(new List<LifecycleStage>() { lifecycle1, lifecycle2 });
            await _dbContext.SaveChangesAsync();
        }

        private Entities.Qualification.QualificationNewReviewRequired CreateNewQualificationsViewData(string qualificationNumber, string qualificationName)
        {
            var orgId = Guid.NewGuid();            
            var qan1_qualification = _fixture.Build<Entities.Qualification.QualificationNewReviewRequired>()                
                .With(w => w.QualificationTitle, qualificationName)
                .With(w => w.QualificationReference, qualificationNumber)
                .Create();

            return qan1_qualification;
        }

        private async Task CreateQualificationRecordSet(int organisationId, string qualificationNumber, string qualificationName)
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
                .With(w => w.LifecycleStageId, LifeCycleStageNew)
                .With(w => w.ProcessStatusId, ProcessStageNoAction)
                .Create();

            var organisations = new List<AwardingOrganisation>() { qan1_organisation };
            var qualifications = new List<Qualification>() { qan1_qualification };
            var qualificationVersions = new List<QualificationVersions>() { qan1_qualificationVersion1 };
            var qualificationVersionFieldChanges = new List<VersionFieldChange>() { qan1_qualificationVersionFieldChange1 };

            await _dbContext.AddRangeAsync(organisations);
            await _dbContext.AddRangeAsync(qualifications);
            await _dbContext.AddRangeAsync(qualificationVersions);
            await _dbContext.AddRangeAsync(qualificationVersionFieldChanges);
            await _dbContext.SaveChangesAsync();
        }


    }
}