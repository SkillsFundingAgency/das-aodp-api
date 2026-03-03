using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review
{
    public class BulkSaveReviewerCommandHandlerTests
    {
        private readonly Mock<IApplicationRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly BulkSaveReviewerCommandHandler _handler;

        private const string ReviewerOld1 = "Old1";
        private const string ReviewerOld2 = "Old2";
        private const string ReviewerNew1 = "New1";
        private const string ReviewerNew2 = "New2";

        private const string SentByName = "Bob";
        private const string SentByEmail = "bob@test.com";

        private const string InvalidUserType = "Invalid";

        public BulkSaveReviewerCommandHandlerTests()
        {
            _handler = new BulkSaveReviewerCommandHandler(_repository.Object, _mediator.Object);
        }

        [Fact]
        public async Task Handle_InvalidUserType_ReturnsError()
        {
            var command = new BulkSaveReviewerCommand
            {
                UserType = "INVALID",
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { Guid.NewGuid() }
            };

            var result = await _handler.Handle(command, default);

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Contains("Invalid User Type", result.ErrorMessage);
            });
        }

        [Fact]
        public async Task Handle_ApplicationMissing_AddsMissingError()
        {
            var id = Guid.NewGuid();

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Data.Entities.Application.Application)null);

            var result = await _handler.Handle(command, default);

            var errors = result.Value.Errors.ToList();

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                var error = Assert.Single(errors);
                Assert.Equal(BulkReviewerErrorType.Missing, error.ErrorType);
                Assert.Equal(id, error.ApplicationId);
            });
        }

        [Fact]
        public async Task Handle_NoReviewerChange_SkipsUpdate()
        {
            var id = Guid.NewGuid();

            var app = new Data.Entities.Application.Application
            {
                Id = id,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2,
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            var result = await _handler.Handle(command, default);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(0, result.Value.UpdatedCount);
                Assert.Equal(0, result.Value.ErrorCount);
                _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            });
        }

        [Fact]
        public async Task Handle_ReviewerConflict_AddsConflictError()
        {
            var id = Guid.NewGuid();

            var app = new Data.Entities.Application.Application
            {
                Id = id,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1 = ReviewerNew1,
                Reviewer2 = ReviewerNew1, // conflict
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            var result = await _handler.Handle(command, default);

            var errors = result.Value.Errors.ToList();

            Assert.Multiple(() =>
            {
                var error = Assert.Single(errors);
                Assert.Equal(BulkReviewerErrorType.Conflict, error.ErrorType);
                Assert.Equal(id, error.ApplicationId);
            });
        }

        [Fact]
        public async Task Handle_UpdatesApplication_WhenReviewersChange()
        {
            var id = Guid.NewGuid();

            var app = new Data.Entities.Application.Application
            {
                Id = id,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1 = ReviewerNew1,
                Reviewer2 = ReviewerOld2,
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            _mediator
                .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(1, result.Value.UpdatedCount);

                _repository.Verify(r =>
                    r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                        a.Id == id &&
                        a.Reviewer1 == ReviewerNew1 &&
                        a.Reviewer2 == ReviewerOld2
                    )),
                    Times.Once);
            });
        }

        [Fact]
        public async Task Handle_MessageSendFails_AddsMessageFailedError()
        {
            var id = Guid.NewGuid();

            var app = new Data.Entities.Application.Application
            {
                Id = id,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1 = ReviewerNew1, // MUST change reviewer
                Reviewer2 = ReviewerOld2,
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            _mediator
                .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = false });

            var result = await _handler.Handle(command, default);

            var errors = result.Value.Errors.ToList();

            Assert.Multiple(() =>
            {
                var error = Assert.Single(errors);
                Assert.Equal(BulkReviewerErrorType.MessageFailed, error.ErrorType);
                Assert.Equal(id, error.ApplicationId);
            });
        }

        [Fact]
        public async Task Handle_SendsCorrectMessageCommand()
        {
            var id = Guid.NewGuid();

            var app = new Data.Entities.Application.Application
            {
                Id = id,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1 = ReviewerNew1,
                Reviewer2 = ReviewerOld2,
                SentByName = SentByName,
                SentByEmail = SentByEmail,
                ApplicationIds = new List<Guid> { id }
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            _mediator
                .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            await _handler.Handle(command, default);

            _mediator.Verify(m =>
                m.Send(
                    It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == id &&
                        c.MessageType == MessageType.QfauOwnerUpdated.ToString() &&
                        c.SentByName == SentByName &&
                        c.SentByEmail == SentByEmail &&
                        c.UserType == UserType.Qfau.ToString()
                    ),
                    default),
                Times.Once);
        }
    }
}
