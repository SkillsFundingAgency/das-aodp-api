using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Jobs;
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
            var result = await _repository.GetJobRunsAsync();

            // Assert
            Assert.Equal(jobRuns.Count, result.Count);
        }

        [Fact]
        public async Task GetJobRunsByName_ReturnsList()
        {
            // Arrange           
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial"})
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.GetJobRunsByNameAsync(jobRuns[0].Job.Name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobRuns.Count, result.Count);
        }

        private async Task PopulateDb(List<JobRun> jobRuns)
        {
            await _dbContext.AddRangeAsync(jobRuns);
            await _dbContext.SaveChangesAsync();
        }
    }
}
