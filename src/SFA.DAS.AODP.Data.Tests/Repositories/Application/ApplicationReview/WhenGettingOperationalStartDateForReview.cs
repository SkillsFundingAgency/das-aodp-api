
using AutoFixture;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Application.ApplicationReview
{
    public class WhenGettingOperationalStartDateForReview
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IApplicationDbContext> _context = new();
        private readonly Data.Repositories.Application.ApplicationReviewRepository _sut;

        public WhenGettingOperationalStartDateForReview() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Returns_Null_When_Application_Has_No_Qualification_Number()
        {
            // Arrange
            var reviewId = Guid.NewGuid();

            var reviews = new List<Entities.Application.ApplicationReview>
            {
                new()
                {
                    Id = reviewId,
                    Application = new Entities.Application.Application
                    {
                        QualificationNumber = null
                    }
                }
            };

            _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet(reviews);

            // Act
            var result = await _sut.GetOperationalStartDateForReview(reviewId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Then_Returns_Null_When_Qualification_Does_Not_Exist()
        {
            // Arrange
            var reviewId = Guid.NewGuid();

            var reviews = new List<Entities.Application.ApplicationReview>
            {
                new()
                {
                    Id = reviewId,
                    Application = new Entities.Application.Application
                    {
                        QualificationNumber = "12345678"
                    }
                }
            };

            _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet(reviews);
            _context.SetupGet(c => c.Qualification)
                .ReturnsDbSet(new List<Qualification>());

            // Act
            var result = await _sut.GetOperationalStartDateForReview(reviewId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task Then_Returns_Operational_Start_Date()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var expectedDate = new DateTime(2024, 1, 1);

            var reviews = new List<Entities.Application.ApplicationReview>
            {
                new()
                {
                    Id = reviewId,
                    Application = new Entities.Application.Application
                    {
                        QualificationNumber = "12345678"
                    }
                }
            };

            var qualifications = new List<Qualification>
            {
                new()
                {
                    Qan = "12345678",
                    QualificationVersions =
                    [
                        new QualificationVersions
                        {
                            Version = 1,
                            OperationalStartDate = expectedDate
                        }
                    ]
                }
            };

            _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet(reviews);
            _context.SetupGet(c => c.Qualification).ReturnsDbSet(qualifications);

            // Act
            var result = await _sut.GetOperationalStartDateForReview(reviewId);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public async Task Then_Returns_Operational_Start_Date_From_Highest_Version()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var expectedDate = new DateTime(2024, 1, 1);

            var reviews = new List<Entities.Application.ApplicationReview>
            {
                new()
                {
                    Id = reviewId,
                    Application = new Entities.Application.Application
                    {
                        QualificationNumber = "12345678"
                    }
                }
            };

            var qualifications = new List<Qualification>
            {
                new()
                {
                    Qan = "12345678",
                    QualificationVersions =
                    [
                        new QualificationVersions
                        {
                            Version = 1,
                            OperationalStartDate = new DateTime(2023, 1, 1)
                        },
                        new QualificationVersions
                        {
                            Version = 2,
                            OperationalStartDate = expectedDate
                        }
                    ]
                }
            };

            _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet(reviews);
            _context.SetupGet(c => c.Qualification).ReturnsDbSet(qualifications);

            // Act
            var result = await _sut.GetOperationalStartDateForReview(reviewId);

            // Assert
            Assert.Equal(expectedDate, result);
        }

    }
}
