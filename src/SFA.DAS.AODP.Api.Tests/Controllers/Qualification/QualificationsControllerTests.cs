using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Qualification;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Models.Qualifications;
using ChangedQualification = SFA.DAS.AODP.Models.Qualifications.ChangedQualification;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Qualification
{
    public class QualificationsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<QualificationsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly QualificationsController _controller;

        public QualificationsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());
            _loggerMock = _fixture.Freeze<Mock<ILogger<QualificationsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new QualificationsController(_mediatorMock.Object, _loggerMock.Object);
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
        public async Task GetQualifications_ReturnsOkResult_WithListOfNewQualifications()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value.Data = _fixture.CreateMany<NewQualification>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "new", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetNewQualificationsQueryResponse>(okResult.Value);
            Assert.Equal(2, model.Data.Count);
            Assert.Equal(queryResponse.Value.Data[0].Title, model.Data[0].Title);
            Assert.Equal(queryResponse.Value.Data[0].Reference, model.Data[0].Reference);
            Assert.Equal(queryResponse.Value.Data[0].AwardingOrganisation, model.Data[0].AwardingOrganisation);
            Assert.Equal(queryResponse.Value.Data[0].Status, model.Data[0].Status);
        }

        [Fact]
        public async Task GetQualifications_ReturnsOkResult_WithListOfChangedQualifications()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value.Data = _fixture.CreateMany<ChangedQualification>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChangedQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "changed", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetChangedQualificationsQueryResponse>(okResult.Value);
            Assert.Equal(2, model.Data.Count);
            Assert.Equal(queryResponse.Value.Data[0].Subject, model.Data[0].Subject);
            Assert.Equal(queryResponse.Value.Data[0].QualificationType, model.Data[0].QualificationType);
            Assert.Equal(queryResponse.Value.Data[0].QualificationReference, model.Data[0].QualificationReference);
            Assert.Equal(queryResponse.Value.Data[0].QualificationTitle, model.Data[0].QualificationTitle);
            Assert.Equal(queryResponse.Value.Data[0].SectorSubjectArea, model.Data[0].SectorSubjectArea);
            Assert.Equal(queryResponse.Value.Data[0].Level, model.Data[0].Level);
            Assert.Equal(queryResponse.Value.Data[0].AwardingOrganisation, model.Data[0].AwardingOrganisation);
            Assert.Equal(queryResponse.Value.Data[0].ChangedFieldNames, model.Data[0].ChangedFieldNames);
        }

        [Fact]
        public async Task GetChangedQualifications_ReturnsStatusCode_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            queryResponse.Success = false;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChangedQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "changed", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var notFoundResult = Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public async Task GetChangedQualifications_ReturnsOk_WhenMediatorReturnsNull()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChangedQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "changed", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
        }



        [Fact]        
        public async Task GetNewQualifications_ReturnsStatusCode_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = false;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "new", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var notFoundResult = Assert.IsType<StatusCodeResult>(result);                   
        }

        [Fact]
        public async Task GetNewQualifications_ReturnsOk_WhenMediatorReturnsNull()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications(status: "new", skip: 0, take: 10, name: "", organisation: "", qan: "");

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);            
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsOkResult_WithQualificationDetails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value.Id, model.Value.Id);
            Assert.Equal(queryResponse.Value.Status, model.Value.Status);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = false;
            queryResponse.ErrorMessage = "No details found for qualification reference: Ref123";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("No details found for qualification reference: Ref123", notFoundValue);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsBadRequest_WhenQualificationReferenceIsEmpty()
        {
            // Act
            var result = await _controller.GetQualificationDetails(string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal("Qualification reference cannot be empty", badRequestValue);
        }

        [Fact]
        public async Task GetQualificationCSVExportData_ReturnsOkResult_WithCSVData()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsCsvExportResponse>>();
            queryResponse.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsCsvExportQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationCSVExportData("new");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetNewQualificationsCsvExportResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value, model.Value);
        }

        [Fact]
        public async Task GetQualificationCSVExportData_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Act
            var result = await _controller.GetQualificationCSVExportData("invalid");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal("Invalid status param: invalid", badRequestValue);
        }

        [Fact]
        public async Task GetQualificationVersionsForQualificationByReference_ReturnsOkResult_WithVersions()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>>();
            queryResponse.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationVersionsForQualificationByReferenceQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationVersionsForQualificationByReference("Ref123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value, model.Value);
        }


        [Fact]
        public async Task GetFeedbackForQualificationFundingById_ReturnsOkResult_WithFeedback()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>>();
            queryResponse.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetFeedbackForQualificationFundingByIdQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetFeedbackForQualificationFundingById(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value, model.Value);
        }


        [Fact]
        public async Task SaveFundingOfferOutcome_ReturnsOkResult()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersOutcomeCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<SaveQualificationsFundingOffersOutcomeCommand>(), default))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.SaveFundingOfferOutcome(command, command.QualificationVersionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task SaveQualificationFundingOffers_ReturnsOkResult()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<SaveQualificationsFundingOffersCommand>(), default))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.SaveQualificationFundingOffers(command, command.QualificationVersionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task SaveQualificationFundingOffersDetails_ReturnsOkResult()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersDetailsCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<SaveQualificationsFundingOffersDetailsCommand>(), default))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.SaveQualificationFundingOffersDetails(command, command.QualificationVersionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task QualificationFundingOffersSummary_ReturnsOkResult()
        {
            // Arrange
            var command = _fixture.Create<CreateQualificationDiscussionHistoryCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateQualificationDiscussionHistoryCommand>(), default))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.QualificationFundingOffersSummary(command, command.QualificationVersionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}


