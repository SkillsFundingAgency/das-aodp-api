using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application
{
    public class GetRelatedQualificationForApplicationQueryHandlerTests
    {
        private readonly Mock<IApplicationRepository> _applicationRepository = new();
        private readonly Mock<IQualificationDetailsRepository> _qualificationsRepository = new();
        private readonly GetRelatedQualificationForApplicationQueryHandler _handler;

        public GetRelatedQualificationForApplicationQueryHandlerTests()
        {
            _handler = new(_applicationRepository.Object, _qualificationsRepository.Object);
        }

        [Fact]
        public async Task Test_Returns_Qualification_Details()
        {
            // Arrange
            var application = new Data.Entities.Application.Application()
            {
                Id = Guid.NewGuid(),
                QualificationNumber = "123"
            };

            var qualification = new QualificationVersions()
            {
                ProcessStatus = new()
                {
                    Name = "1",
                },
                Name = "2",
                Qualification = new()
                {
                    Qan = "123",

                }
            };

            _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).ReturnsAsync(application);
            _qualificationsRepository.Setup(a => a.GetQualificationDetailsByIdAsync(application.QualificationNumber)).ReturnsAsync(qualification);

            // Act
            var response = await _handler.Handle(new(application.Id), default);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Value);

            Assert.Equal(qualification.ProcessStatus.Name, response.Value.Status);
            Assert.Equal(qualification.Name, response.Value.Name);
            Assert.Equal(qualification.Qualification.Qan, response.Value.Qan);
            _applicationRepository.Verify(a => a.GetByIdAsync(application.Id), Times.Once());
            _qualificationsRepository.Verify(a => a.GetQualificationDetailsByIdAsync(application.QualificationNumber), Times.Once());
        }


        [Fact]
        public async Task Test_Exception_Thrown_For_Invalid_Qan()
        {
            // Arrange
            var application = new Data.Entities.Application.Application()
            {
                Id = Guid.NewGuid(),
                QualificationNumber = string.Empty
            };
            _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).ReturnsAsync(application);

            // Act
            var response = await _handler.Handle(new(application.Id), default);

            // Assert
            Assert.False(response.Success);
            Assert.IsAssignableFrom<Exception>(response.InnerException);
            Assert.Equal("No QAN has been provided for the application", response.InnerException.Message);
        }

    }
}