using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetQualificationDetailsQueryHandlerTests
    {
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly GetQualificationDetailsQueryHandler _handler;

        public GetQualificationDetailsQueryHandlerTests()
        {
            _repositoryMock = new Mock<INewQualificationsRepository>();
            _handler = new GetQualificationDetailsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var qualificationReference = "QUAL123";
            var qualificationDetails = new QualificationDetails
            {
                Id = 1,
                Status = "Active",
                Priority = "High",
                Changes = "None",
                QualificationReference = qualificationReference,
                AwardingOrganisation = "Org1",
                Title = "Qualification Title",
                QualificationType = "Type1",
                Level = "Level1",
                ProposedChanges = "None",
                AgeGroup = "16-18",
                Category = "Category1",
                Subject = "Subject1",
                SectorSubjectArea = "Area1",
                Comments = "No comments"
            };

            _repositoryMock.Setup(repo => repo.GetQualificationDetailsByIdAsync(qualificationReference))
                .ReturnsAsync(qualificationDetails);

            var request = new GetQualificationDetailsQuery { QualificationReference = qualificationReference };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(qualificationDetails.Id, response.Id);
            Assert.Equal(qualificationDetails.Status, response.Status);
            Assert.Equal(qualificationDetails.Priority, response.Priority);
            Assert.Equal(qualificationDetails.Changes, response.Changes);
            Assert.Equal(qualificationDetails.QualificationReference, response.QualificationReference);
            Assert.Equal(qualificationDetails.AwardingOrganisation, response.AwardingOrganisation);
            Assert.Equal(qualificationDetails.Title, response.Title);
            Assert.Equal(qualificationDetails.QualificationType, response.QualificationType);
            Assert.Equal(qualificationDetails.Level, response.Level);
            Assert.Equal(qualificationDetails.ProposedChanges, response.ProposedChanges);
            Assert.Equal(qualificationDetails.AgeGroup, response.AgeGroup);
            Assert.Equal(qualificationDetails.Category, response.Category);
            Assert.Equal(qualificationDetails.Subject, response.Subject);
            Assert.Equal(qualificationDetails.SectorSubjectArea, response.SectorSubjectArea);
            Assert.Equal(qualificationDetails.Comments, response.Comments);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_No_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var qualificationReference = "QUAL123";

            _repositoryMock.Setup(repo => repo.GetQualificationDetailsByIdAsync(qualificationReference))
                .ReturnsAsync((QualificationDetails?)null);

            var request = new GetQualificationDetailsQuery { QualificationReference = qualificationReference };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(response.Success);
        }
    }
}
