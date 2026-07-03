using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Rollover;
using SFA.DAS.AODP.Shared.UnitTests.Helpers;
using static SFA.DAS.AODP.Application.Queries.Qualification.GetQualificationVersionsForQualificationByReferenceQueryResponse;
using QualificationVersions = SFA.DAS.AODP.Data.Entities.Qualification.QualificationVersions;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories
{
    public class QualificationFundingsRepositoryTests
    {
        private readonly IFixture _fixture;

        public QualificationFundingsRepositoryTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());
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

            var fundings = CreateFundings(2, versionId);

            context.AddRange(
                fundings[0].QualificationVersion.Qualification,
                fundings[0].QualificationVersion,
                fundings[0].FundingOffer,
                fundings[0],
                fundings[1]);

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

            var versionId = Guid.NewGuid();
            var fundings = CreateFundings(1, versionId);

            context.QualificationFundings.Add(fundings[0]);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            fundings[0].Comments = "Updated";

            // Act
            await repo.UpdateAsync(new List<QualificationFundings> { fundings[0] });

            // Assert
            var updated = context.QualificationFundings.First();
            Assert.Equal("Updated", updated.Comments);
        }

        [Fact]
        public async Task RemoveAsync_RemovesFundings()
        {
            using var context = CreateInMemoryContext();

            var versionId = Guid.NewGuid();

            var fundings = CreateFundings(1, versionId);
            context.QualificationFundings.Add(fundings[0]);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            // Act
            await repo.RemoveAsync(new List<QualificationFundings> { fundings[0] });

            // Assert
            Assert.Empty(context.QualificationFundings);
        }

        [Fact]
        public async Task GetRolloverQualificationFundingsAsync_ReturnsMatchingKeys()
        {
            using var context = CreateInMemoryContext();

            var versionId = Guid.NewGuid();
            var fundingList = CreateFundings(3, versionId);

            context.QualificationFundings.AddRange(fundingList);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repo = new QualificationFundingsRepository(context);

            var keys = new List<QualificationFundingKey>
            {
                new(versionId, fundingList[0].FundingOfferId),
                new(versionId, fundingList[1].FundingOfferId)
            };

            // Act
            var result = await repo.GetRolloverQualificationFundingsAsync(keys, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Id == fundingList[0].Id);
            Assert.Contains(result, x => x.Id == fundingList[1].Id);
            Assert.DoesNotContain(result, x => x.Id == fundingList[2].Id);
        }

        private List<QualificationFundings> CreateFundings(int count, Guid versionId)
        {
            DateTime now = DateTime.UtcNow;

            List<QualificationFundings> fundings = new();

            var qualification = new Qualification
            {
                Id = Guid.NewGuid(),
                QualificationName = "Test Qualification",
                Qan = "XXXX1234"
            };

            var qualificationVersion = new QualificationVersions
            {
                QualificationId = qualification.Id,
                Qualification = qualification,
                Version = 1,
                EqfLevel = "Efq level example",
                Level = "Level example",
                Ssa = "Ssa example",
                Status = "Status example",
                SubLevel = "Sub level example",
                Type = "Type example",
                Id = versionId
            };

            for (int i = 0; i < count; i++)
            {
                var fundingOffer = new FundingOffer
                {
                    Id = Guid.NewGuid(),
                    Name = $"Test Funding Offer {i}"
                };

                var funding = new QualificationFundings
                {
                    Id = Guid.NewGuid(),
                    QualificationVersionId = qualificationVersion.Id,
                    QualificationVersion = qualificationVersion,
                    FundingOfferId = fundingOffer.Id,
                    FundingOffer = fundingOffer,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
                };

                fundings.Add(funding);
            }

            return fundings;
        }
    }
}
