using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories
{
    public class QualificationFundingsRepositoryTests
    {
        private readonly IFixture _fixture;

        public QualificationFundingsRepositoryTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMatchingFundings()
        {
            using var context = CreateInMemoryContext();

            var versionId = Guid.NewGuid();

            var funding1 = _fixture.Build<QualificationFundings>()
                .With(x => x.QualificationVersionId, versionId)
                .Create();

            var funding2 = _fixture.Build<QualificationFundings>()
                .With(x => x.QualificationVersionId, versionId)
                .Create();

            context.QualificationFundings.AddRange(funding1, funding2);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            // Act
            var result = await repo.GetByIdAsync(versionId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(versionId, x.QualificationVersionId));
        }


        [Fact]
        public async Task CreateAsync_AddsFundings()
        {
            using var context = CreateInMemoryContext();
            var repo = new QualificationFundingsRepository(context);

            var items = _fixture.CreateMany<QualificationFundings>(3).ToList();

            // Act
            await repo.CreateAsync(items);

            // Assert
            Assert.Equal(3, context.QualificationFundings.Count());
            Assert.All(context.QualificationFundings, x => Assert.NotEqual(Guid.Empty, x.Id));
        }


        [Fact]
        public async Task UpdateAsync_UpdatesExistingFundings()
        {
            using var context = CreateInMemoryContext();

            var funding = _fixture.Create<QualificationFundings>();
            context.QualificationFundings.Add(funding);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            funding.Comments = "Updated";

            // Act
            await repo.UpdateAsync(new List<QualificationFundings> { funding });

            // Assert
            var updated = context.QualificationFundings.First();
            Assert.Equal("Updated", updated.Comments);
        }

        [Fact]
        public async Task RemoveAsync_RemovesFundings()
        {
            using var context = CreateInMemoryContext();

            var funding = _fixture.Create<QualificationFundings>();
            context.QualificationFundings.Add(funding);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            // Act
            await repo.RemoveAsync(new List<QualificationFundings> { funding });

            // Assert
            Assert.Empty(context.QualificationFundings);
        }

        [Fact]
        public async Task GetRolloverQualificationFundingsAsync_ReturnsMatchingKeys()
        {
            using var context = CreateInMemoryContext();

            var f1 = _fixture.Create<QualificationFundings>();
            var f2 = _fixture.Create<QualificationFundings>();
            var f3 = _fixture.Create<QualificationFundings>(); // non-match

            context.QualificationFundings.AddRange(f1, f2, f3);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            var keys = new List<QualificationFundingKey>
            {
                new(f1.QualificationVersionId, f1.FundingOfferId),
                new(f2.QualificationVersionId, f2.FundingOfferId)
            };

            // Act
            var result = await repo.GetRolloverQualificationFundingsAsync(keys, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Id == f1.Id);
            Assert.Contains(result, x => x.Id == f2.Id);
            Assert.DoesNotContain(result, x => x.Id == f3.Id);
        }
    }
}
