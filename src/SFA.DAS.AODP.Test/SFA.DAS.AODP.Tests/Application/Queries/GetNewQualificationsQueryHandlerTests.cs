using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetNewQualificationsQueryHandlerTests
    {
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly GetNewQualificationsQueryHandler _handler;

        public GetNewQualificationsQueryHandlerTests()
        {
            _repositoryMock = new Mock<INewQualificationsRepository>();
            _handler = new GetNewQualificationsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_And_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var newQualifications = new List<NewQualification>
        {
            new NewQualification { Id = 1, Title = "Qualification 1", Reference = "REF1", AwardingOrganisation = "Org1", Status = "Active" },
            new NewQualification { Id = 2, Title = "Qualification 2", Reference = "REF2", AwardingOrganisation = "Org2", Status = "Inactive" }
        };

            _repositoryMock.Setup(repo => repo.GetAllNewQualificationsAsync())
                .ReturnsAsync(newQualifications);

            var request = new GetNewQualificationsQuery();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(newQualifications, response.NewQualifications);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_And_No_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var newQualifications = new List<NewQualification>();

            _repositoryMock.Setup(repo => repo.GetAllNewQualificationsAsync())
                .ReturnsAsync(newQualifications);

            var request = new GetNewQualificationsQuery();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.NewQualifications);
        }
    }
}
