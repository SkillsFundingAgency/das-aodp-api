using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetNewQualificationsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly GetNewQualificationsQueryHandler _handler;

        public GetNewQualificationsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _repositoryMock = _fixture.Freeze<Mock<INewQualificationsRepository>>();

            _handler = _fixture.Create<GetNewQualificationsQueryHandler>();
        }

        [Fact]
        public async Task Returns_success_and_data()
        {
            var query = new GetNewQualificationsQuery
            {
                Skip = 10,
                Take = 20
            };

            var repoResult = new NewQualificationsResult
            {
                TotalRecords = 2,
                Data = new List<NewQualification>
                {
                    new NewQualification(),
                    new NewQualification()
                }
            };

            _repositoryMock
                .Setup(x => x.GetAllNewQualificationsAsync(
                    query.Skip,
                    query.Take,
                    It.IsAny<NewQualificationsFilter>()))
                .ReturnsAsync(repoResult);

            var result = await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.Verify(x =>
                x.GetAllNewQualificationsAsync(query.Skip, query.Take, It.IsAny<NewQualificationsFilter>()),
                Times.Once);

            Assert.True(result.Success);
            Assert.Equal(2, result.Value.Data.Count);
            Assert.Equal(2, result.Value.TotalRecords);
        }

        [Fact]
        public async Task Returns_success_even_when_empty()
        {
            var query = new GetNewQualificationsQuery
            {
                Skip = 10,
                Take = 20
            };

            _repositoryMock
                .Setup(x => x.GetAllNewQualificationsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<NewQualificationsFilter>()))
                .ReturnsAsync(new NewQualificationsResult
                {
                    Data = new List<NewQualification>(),
                    TotalRecords = 0
                });

            var result = await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.Verify(x =>
                x.GetAllNewQualificationsAsync(query.Skip, query.Take, It.IsAny<NewQualificationsFilter>()),
                Times.Once);

            Assert.True(result.Success);
            Assert.Empty(result.Value.Data);
        }

        [Fact]
        public async Task Handles_exception_and_returns_failure()
        {
            var query = new GetNewQualificationsQuery
            {
                Skip = 10,
                Take = 20
            };

            _repositoryMock
                .Setup(x => x.GetAllNewQualificationsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<NewQualificationsFilter>()))
                .ThrowsAsync(new Exception("boom"));

            var result = await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.Verify(x =>
                x.GetAllNewQualificationsAsync(query.Skip, query.Take, It.IsAny<NewQualificationsFilter>()),
                Times.Once);

            Assert.False(result.Success);
            Assert.Equal("boom", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_Passes_ProcessStatusFilter_And_AgeGroups_To_Repository()
        {
            // Arrange
            var processStatuses = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var ageGroups = new List<AgeGroup> { AgeGroup.Pre16, AgeGroup.NineteenPlus };

            var query = new GetNewQualificationsQuery
            {
                Skip = 0,
                Take = 10,
                ProcessStatusIds = processStatuses,
                AgeGroups = ageGroups
            };

            NewQualificationsFilter? capturedFilter = null;


            _repositoryMock
                .Setup(x => x.GetAllNewQualificationsAsync(
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<NewQualificationsFilter>()))
                .Callback<int?, int?, NewQualificationsFilter>((_, _, f) => capturedFilter = f)
                .ReturnsAsync(new NewQualificationsResult
                {
                    Data = new List<NewQualification>(),
                    TotalRecords = 0
                });


            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedFilter);
            Assert.Equal(processStatuses, capturedFilter!.ProcessStatusIds);
            Assert.Equal(ageGroups, capturedFilter.AgeGroups);
        }
    }
}