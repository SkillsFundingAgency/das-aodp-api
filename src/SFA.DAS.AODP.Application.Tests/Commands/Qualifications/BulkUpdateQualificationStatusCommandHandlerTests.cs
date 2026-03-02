using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using static SFA.DAS.AODP.Data.Repositories.Qualification.IQualificationsRepository;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class BulkUpdateQualificationStatusCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationsRepository> _qualificationsRepositoryMock;
        private readonly BulkUpdateQualificationStatusCommandHandler _handler;

        private const string MissingTitle = "Qualification not found";
        private const string StatusUpdateFailedTitle = "Failed to update status";
        private const string HistoryFailedTitle = "Status updated but failed to add history";

        public BulkUpdateQualificationStatusCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _qualificationsRepositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = new BulkUpdateQualificationStatusCommandHandler(_qualificationsRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WithMappedCountsAndErrors()
        {
            var command = _fixture.Create<BulkUpdateQualificationStatusCommand>();
            command.QualificationIds.Add(Guid.Empty);
            command.QualificationIds.Add(command.QualificationIds[0]);

            var missingId = Guid.NewGuid();

            var statusFailedQualificationId = Guid.NewGuid();
            var statusFailedQan = "QAN-STATUS-FAIL";

            var historyFailedQualificationId = Guid.NewGuid();
            var historyFailedQan = "QAN-HISTORY-FAIL";

            var statusId = Guid.NewGuid();
            var statusName = "In Review";

            var repoResult = new BulkUpdateQualificationStatusWithHistoryResult
            {
                Status = new ProcessStatus { Id = statusId, Name = statusName },
                Succeeded = new List<(Guid QualificationVersionId, string Qan)>
                {
                    (Guid.NewGuid(), "QAN-OK-1"),
                    (Guid.NewGuid(), "QAN-OK-2")
                },
                MissingIds = new List<Guid> { missingId },
                StatusUpdateFailed = new List<(Guid QualificationId, string Qan, string Message)>
                {
                    (statusFailedQualificationId, statusFailedQan, "ignored")
                },
                HistoryFailed = new List<(Guid QualificationId, string Qan, string Message)>
                {
                    (historyFailedQualificationId, historyFailedQan, "ignored")
                }
            };

            _qualificationsRepositoryMock
                .Setup(r => r.BulkUpdateQualificationStatusWithHistoryAsync(
                    It.IsAny<IReadOnlyCollection<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(statusId, result.Value!.ProcessStatusId);
            Assert.Equal(statusName, result.Value.ProcessStatusName);

            var expectedRequestedCount = command.QualificationIds.Where(x => x != Guid.Empty).Distinct().Count();
            Assert.Equal(expectedRequestedCount, result.Value.RequestedCount);

            Assert.Equal(repoResult.Succeeded.Count, result.Value.UpdatedCount);
            Assert.Equal(3, result.Value.ErrorCount);
            Assert.Equal(3, result.Value.Errors.Count);

            var missingError = result.Value.Errors.Single(e => e.ErrorType == BulkQualificationErrorType.Missing);
            Assert.Equal(missingId, missingError.QualificationId);
            Assert.Equal(string.Empty, missingError.Qan);
            Assert.Equal(MissingTitle, missingError.Title);

            var statusFailedError = result.Value.Errors.Single(e => e.ErrorType == BulkQualificationErrorType.StatusUpdateFailed);
            Assert.Equal(statusFailedQualificationId, statusFailedError.QualificationId);
            Assert.Equal(statusFailedQan, statusFailedError.Qan);
            Assert.Equal(StatusUpdateFailedTitle, statusFailedError.Title);

            var historyFailedError = result.Value.Errors.Single(e => e.ErrorType == BulkQualificationErrorType.HistoryFailed);
            Assert.Equal(historyFailedQualificationId, historyFailedError.QualificationId);
            Assert.Equal(historyFailedQan, historyFailedError.Qan);
            Assert.Equal(HistoryFailedTitle, historyFailedError.Title);

            _qualificationsRepositoryMock.Verify(r => r.BulkUpdateQualificationStatusWithHistoryAsync(
                    command.QualificationIds,
                    command.ProcessStatusId,
                    command.UserDisplayName,
                    command.Comment,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WithNoErrors_WhenNoMissingOrFailed()
        {
            var command = _fixture.Create<BulkUpdateQualificationStatusCommand>();

            var statusId = Guid.NewGuid();
            var statusName = "Approved";

            var repoResult = new BulkUpdateQualificationStatusWithHistoryResult
            {
                Status = new ProcessStatus { Id = statusId, Name = statusName },
                Succeeded = new List<(Guid QualificationVersionId, string Qan)>
                {
                    (Guid.NewGuid(), "QAN-OK-1")
                },
                MissingIds = Array.Empty<Guid>(),
                StatusUpdateFailed = Array.Empty<(Guid QualificationId, string Qan, string Message)>(),
                HistoryFailed = Array.Empty<(Guid QualificationId, string Qan, string Message)>()
            };

            _qualificationsRepositoryMock
                .Setup(r => r.BulkUpdateQualificationStatusWithHistoryAsync(
                    It.IsAny<IReadOnlyCollection<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(0, result.Value!.ErrorCount);
            Assert.Empty(result.Value.Errors);
            Assert.Equal(repoResult.Succeeded.Count, result.Value.UpdatedCount);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenNoForeignKeyExceptionIsThrown()
        {
            var command = _fixture.Create<BulkUpdateQualificationStatusCommand>();
            var fkId = command.ProcessStatusId;

            _qualificationsRepositoryMock
                .Setup(r => r.BulkUpdateQualificationStatusWithHistoryAsync(
                    It.IsAny<IReadOnlyCollection<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NoForeignKeyException(fkId));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.IsType<DependantNotFoundException>(result.InnerException);

            var inner = (DependantNotFoundException)result.InnerException!;
            Assert.Equal(command.ProcessStatusId, inner.DependantId);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            var command = _fixture.Create<BulkUpdateQualificationStatusCommand>();
            var exception = new Exception("Test exception");

            _qualificationsRepositoryMock
                .Setup(r => r.BulkUpdateQualificationStatusWithHistoryAsync(
                    It.IsAny<IReadOnlyCollection<Guid>>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }
    }
}