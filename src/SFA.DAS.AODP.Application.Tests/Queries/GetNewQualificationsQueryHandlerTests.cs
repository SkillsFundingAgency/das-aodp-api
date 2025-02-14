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
            _handler = new GetNewQualificationsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_And_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var newQualifications = _fixture.CreateMany<NewQualification>(2).ToList();

            _repositoryMock.Setup(repo => repo.GetAllNewQualificationsAsync())
                .ReturnsAsync(newQualifications);

            var request = _fixture.Create<GetNewQualificationsQuery>();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(newQualifications, response.Value.NewQualifications);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_And_No_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var newQualifications = new List<NewQualification>();

            _repositoryMock.Setup(repo => repo.GetAllNewQualificationsAsync())
                .ReturnsAsync(newQualifications);

            var request = _fixture.Create<GetNewQualificationsQuery>();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("No new qualifications found.", response.ErrorMessage);
            Assert.Empty(response.Value.NewQualifications);
        }
    }
}

