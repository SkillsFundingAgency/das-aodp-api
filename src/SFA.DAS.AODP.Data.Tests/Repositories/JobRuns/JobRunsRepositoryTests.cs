using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Enum;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Data.Tests.Repositories.JobRuns
{
    public class JobRunsRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private JobRunsRepository _repository;
        private Fixture _fixture;

        public JobRunsRepositoryTests()
        {
            _fixture = new Fixture();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ApplicationDbContext" + Guid.NewGuid()).Options;
            var configuration = new Mock<IConfiguration>();
            _dbContext = new ApplicationDbContext(options, configuration.Object);
            _repository = new JobRunsRepository(_dbContext);
        }       

        [Fact]
        public async Task GetAllJobRuns_ReturnsList()
        {
            // Arrange           
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial"})
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.GetJobRunsAsync("TestJob");

            // Assert
            Assert.Equal(jobRuns.Count, result.Count);
        }

        [Fact]
        public async Task GetJobRunsById_ReturnsList()
        {
            // Arrange           
            var id = Guid.NewGuid();
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial"})
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.GetJobRunsById(jobRuns[0].Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobRuns[0].Id, result.Id);
        }

        [Fact]
        public async Task RequestJobRun_ReturnsTrue()
        {
            // Arrange           
            var id = Guid.NewGuid();
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial" })
                .With(w => w.Status, JobStatus.Completed.ToString())
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.RequestJobRun("TestJob", "TestUser");

            // Assert
            Assert.True(result);  
            var resultJobRuns = _dbContext.JobRuns.Where(w => w.Status == JobStatus.Requested.ToString()).ToList();
            Assert.Single(resultJobRuns);
        }

        [Fact]
        public async Task RequestJobRun_AlreadyRunning()
        {
            // Arrange           
            var id = Guid.NewGuid();
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial" })
                .With(w => w.Status, JobStatus.Requested.ToString())
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.RequestJobRun("TestJob", "TestUser");

            // Assert
            Assert.True(result);
            var resultJobRuns = _dbContext.JobRuns.Where(w => w.Status == JobStatus.Requested.ToString()).ToList();
            Assert.Equal(3, resultJobRuns.Count());
        }

        private async Task PopulateDb(List<JobRun> jobRuns)
        {
            await _dbContext.AddRangeAsync(jobRuns);
            await _dbContext.SaveChangesAsync();
        }
    }
}
