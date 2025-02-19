using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Data.Tests
{
    public class JobsRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private JobsRepository _repository;       
        private Fixture _fixture;

        public JobsRepositoryTests()
        {
            _fixture = new Fixture();          
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ApplicationDbContext" + Guid.NewGuid()).Options;
            var configuration = new Mock<IConfiguration>();
            _dbContext = new ApplicationDbContext(options, configuration.Object);
            _repository = new JobsRepository(_dbContext);
        }

        [Fact]
        public async Task GetAllJobs_ReturnsList()
        {
            // Arrange           
            var jobs = _fixture.Build<Job>()
                .With(w => w.JobRuns, new List<JobRun>())
                .With(w => w.JobConfigurations, new List<JobConfiguration>())
                .CreateMany<Job>(3)
                .ToList();
            
            await PopulateDb(jobs);

            // Act
            var result = await _repository.GetJobsAsync();

            // Assert
            Assert.Equal(jobs.Count, result.Count);
        }

        [Fact]
        public async Task GetJobByName_ReturnsJob()
        {
            // Arrange           
            var jobs = _fixture.Build<Job>()
                .With(w => w.JobRuns, new List<JobRun>())
                .With(w => w.JobConfigurations, new List<JobConfiguration>())
                .CreateMany<Job>(3)
                .ToList();

            await PopulateDb(jobs);

            // Act
            var result = await _repository.GetJobByNameAsync(jobs[0].Name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobs[0].Id, result.Id);
            Assert.Equal(jobs[0].Name, result.Name);
        }

        [Fact]
        public async Task GetJobById_ReturnsJob()
        {
            // Arrange           
            var jobs = _fixture.Build<Job>()
                .With(w => w.JobRuns, new List<JobRun>())
                .With(w => w.JobConfigurations, new List<JobConfiguration>())
                .CreateMany<Job>(3)
                .ToList();

            await PopulateDb(jobs);

            // Act
            var result = await _repository.GetJobByIdAsync(jobs[0].Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobs[0].Id, result.Id);
            Assert.Equal(jobs[0].Name, result.Name);
        }

        private async Task PopulateDb(List<Job> jobs)
        {
            await _dbContext.AddRangeAsync(jobs);
            await _dbContext.SaveChangesAsync();
        }
    }
}