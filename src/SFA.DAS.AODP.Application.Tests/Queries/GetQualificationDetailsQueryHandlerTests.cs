using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetQualificationDetailsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationsRepository> _repositoryMock;
        private readonly GetQualificationDetailsQueryHandler _handler;

        public GetQualificationDetailsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = _fixture.Create<GetQualificationDetailsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            response.Success = true;

            _repositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(new QualificationDetails
                           {
                               Id = response.Value.Id,
                               Status = response.Value.Status,
                               Priority = response.Value.Priority,
                               Changes = response.Value.Changes,
                               QualificationReference = response.Value.QualificationReference,
                               AwardingOrganisation = response.Value.AwardingOrganisation,
                               Title = response.Value.Title,
                               QualificationType = response.Value.QualificationType,
                               Level = response.Value.Level,
                               ProposedChanges = response.Value.ProposedChanges,
                               AgeGroup = response.Value.AgeGroup,
                               Category = response.Value.Category,
                               Subject = response.Value.Subject,
                               SectorSubjectArea = response.Value.SectorSubjectArea,
                               Comments = response.Value.Comments
                           });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Value.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Failure_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var baseResponse = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>
            {
                Success = false,
                Value = null,
                ErrorMessage = $"No details found for qualification reference: {query.QualificationReference}"
            };

            _repositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync((QualificationDetails?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal($"No details found for qualification reference: {query.QualificationReference}", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var exceptionMessage = "An error occurred";
            _repositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


