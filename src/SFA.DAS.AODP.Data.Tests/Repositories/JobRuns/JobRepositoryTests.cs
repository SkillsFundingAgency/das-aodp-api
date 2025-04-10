using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Data.Tests.Repositories.JobRuns
{
    public class JobRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private JobsRepository _repository;
        private Fixture _fixture;

        public JobRepositoryTests()
        {
            _fixture = new Fixture();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ApplicationDbContext" + Guid.NewGuid()).Options;
            _dbContext = new ApplicationDbContext(options);
            _repository = new JobsRepository(_dbContext);
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
            var result = await _repository.GetJobsAsync();

            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal("TestJob", result[0].Name);
        }

        [Fact]
        public async Task GetJobByName_ReturnsList()
        {
            // Arrange           
            var id = Guid.NewGuid();
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial", Id = id})
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.GetJobByNameAsync("TestJob");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetJobById_ReturnsList()
        {
            // Arrange           
            var id = Guid.NewGuid();
            var jobRuns = _fixture.Build<JobRun>()
                .With(w => w.Job, new Job() { Name = "TestJob", Status = "Initial", Id = id })
                .CreateMany<JobRun>(3)
                .ToList();

            await PopulateDb(jobRuns);

            // Act
            var result = await _repository.GetJobByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        private async Task PopulateDb(List<JobRun> jobRuns)
        {
            await _dbContext.AddRangeAsync(jobRuns);
            await _dbContext.SaveChangesAsync();
        }
    }
}
