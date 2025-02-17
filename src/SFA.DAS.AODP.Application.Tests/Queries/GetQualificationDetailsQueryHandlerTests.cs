using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
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
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly GetQualificationDetailsQueryHandler _handler;

        public GetQualificationDetailsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<INewQualificationsRepository>>();
            _handler = new GetQualificationDetailsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var qualificationDetails = _fixture.Create<QualificationDetails>();
            var qualificationReference = qualificationDetails.QualificationReference;

            _repositoryMock.Setup(repo => repo.GetQualificationDetailsByIdAsync(qualificationReference))
                .ReturnsAsync(qualificationDetails);

            var request = _fixture.Build<GetQualificationDetailsQuery>()
                .With(q => q.QualificationReference, qualificationReference)
                .Create();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(qualificationDetails.Id, response.Value.Id);
            Assert.Equal(qualificationDetails.Status, response.Value.Status);
            Assert.Equal(qualificationDetails.Priority, response.Value.Priority);
            Assert.Equal(qualificationDetails.Changes, response.Value.Changes);
            Assert.Equal(qualificationDetails.QualificationReference, response.Value.QualificationReference);
            Assert.Equal(qualificationDetails.AwardingOrganisation, response.Value.AwardingOrganisation);
            Assert.Equal(qualificationDetails.Title, response.Value.Title);
            Assert.Equal(qualificationDetails.QualificationType, response.Value.QualificationType);
            Assert.Equal(qualificationDetails.Level, response.Value.Level);
            Assert.Equal(qualificationDetails.ProposedChanges, response.Value.ProposedChanges);
            Assert.Equal(qualificationDetails.AgeGroup, response.Value.AgeGroup);
            Assert.Equal(qualificationDetails.Category, response.Value.Category);
            Assert.Equal(qualificationDetails.Subject, response.Value.Subject);
            Assert.Equal(qualificationDetails.SectorSubjectArea, response.Value.SectorSubjectArea);
            Assert.Equal(qualificationDetails.Comments, response.Value.Comments);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_No_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var qualificationReference = _fixture.Create<string>();

            _repositoryMock.Setup(repo => repo.GetQualificationDetailsByIdAsync(qualificationReference))
                .ReturnsAsync((QualificationDetails?)null);

            var request = _fixture.Build<GetQualificationDetailsQuery>()
                .With(q => q.QualificationReference, qualificationReference)
                .Create();

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Qualification not found", response.ErrorMessage);
        }
    }
}

