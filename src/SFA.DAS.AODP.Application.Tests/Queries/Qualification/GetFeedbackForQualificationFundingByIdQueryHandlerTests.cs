using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetFeedbackForQualificationFundingByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationFundingFeedbackRepository> _qualificationFundingFeedbackRepositoryMock;
        private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsRepositoryMock;
        private readonly GetFeedbackForQualificationFundingByIdQueryHandler _handler;

        public GetFeedbackForQualificationFundingByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _qualificationFundingFeedbackRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingFeedbackRepository>>();
            _qualificationFundingsRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingsRepository>>();
            _handler = new GetFeedbackForQualificationFundingByIdQueryHandler(
                _qualificationFundingFeedbackRepositoryMock.Object,
                _qualificationFundingsRepositoryMock.Object);
        }

        public class DateOnlySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (request is Type type && type == typeof(DateOnly))
                {
                    return DateOnly.FromDateTime(DateTime.Now);
                }

                return new NoSpecimen();
            }
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsFound()
        {
            // Arrange
            var query = _fixture.Create<GetFeedbackForQualificationFundingByIdQuery>();
            var feedback = _fixture.Create<QualificationFundingFeedbacks>();
            var fundings = _fixture.CreateMany<QualificationFundings>(2).ToList();

            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationVersionId))
                .ReturnsAsync(feedback);
            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationVersionId))
                .ReturnsAsync(fundings);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(feedback.QualificationVersionId, result.Value.QualificationVersionId);
            Assert.Equal(fundings.Count, result.Value.QualificationFundedOffers.Count);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenFeedbackIsNull()
        {
            // Arrange
            var query = _fixture.Create<GetFeedbackForQualificationFundingByIdQuery>();
            var fundings = _fixture.CreateMany<QualificationFundings>(2).ToList();

            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationVersionId))
                .ReturnsAsync((QualificationFundingFeedbacks)null);
            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationVersionId))
                .ReturnsAsync(fundings);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(query.QualificationVersionId, result.Value.QualificationVersionId);
            Assert.Equal(fundings.Count, result.Value.QualificationFundedOffers.Count);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var query = _fixture.Create<GetFeedbackForQualificationFundingByIdQuery>();
            var exception = new Exception("Test exception");

            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationVersionId))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }
    }
}
