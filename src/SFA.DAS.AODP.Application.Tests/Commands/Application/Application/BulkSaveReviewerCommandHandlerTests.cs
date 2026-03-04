using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Exceptions;
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
                ApplicationIds = new List<Guid> { Guid.NewGuid() },
                SentByEmail = SentByEmail,
                SentByName = SentByName
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
                ApplicationIds = new List<Guid> { id },
                SentByEmail = SentByEmail,
                SentByName = SentByName
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new RecordNotFoundException(id));

            var result = await _handler.Handle(command, default);

            var error = Assert.Single(result.Value.Errors);
            Assert.Multiple(() =>
            {
                Assert.Equal(BulkReviewerErrorType.Missing, error.ErrorType);
                Assert.Equal(id, error.ApplicationId);
            });
        }

        [Fact]
        public async Task Handle_NoReviewerSet_DoesNothing()
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
                Reviewer1Set = false,
                Reviewer2Set = false,
                ApplicationIds = new List<Guid> { id },
                SentByEmail = SentByEmail,
                SentByName = SentByName
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
        public async Task Handle_Reviewer1Updated_WhenReviewer1Set()
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
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                Reviewer2Set = false,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == ReviewerNew1 &&
                    a.Reviewer2 == ReviewerOld2
                )),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Reviewer1Cleared_WhenReviewer1SetAndNull()
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
                Reviewer1Set = true,
                Reviewer1 = null,
                Reviewer2Set = false,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == null &&
                    a.Reviewer2 == ReviewerOld2
                )),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Reviewer2Updated_WhenReviewer2Set()
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
                Reviewer1Set = false,
                Reviewer2Set = true,
                Reviewer2 = ReviewerNew2,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == ReviewerOld1 &&
                    a.Reviewer2 == ReviewerNew2
                )),
                Times.Once);
        }

        
        [Fact]
        public async Task Handle_Reviewer2Cleared_WhenReviewer2SetAndNull()
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
                Reviewer1Set = false,
                Reviewer2Set = true,
                Reviewer2 = null,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == ReviewerOld1 &&
                    a.Reviewer2 == null
                )),
                Times.Once);
        }

        [Fact]
        public async Task Handle_BothReviewersUpdated()
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
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                Reviewer2Set = true,
                Reviewer2 = ReviewerNew2,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == ReviewerNew1 &&
                    a.Reviewer2 == ReviewerNew2
                )),
                Times.Once);
        }

        [Fact]
        public async Task Handle_BothReviewersCleared()
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
                Reviewer1Set = true,
                Reviewer1 = null,
                Reviewer2Set = true,
                Reviewer2 = null,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true });

            var result = await _handler.Handle(command, default);

            Assert.Equal(1, result.Value.UpdatedCount);

            _repository.Verify(r =>
                r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                    a.Reviewer1 == null &&
                    a.Reviewer2 == null
                )),
                Times.Once);
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
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                Reviewer2Set = true,
                Reviewer2 = ReviewerNew1, // conflict
                ApplicationIds = new List<Guid> { id },
                SentByEmail = SentByEmail,
                SentByName = SentByName
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);

            var result = await _handler.Handle(command, default);

            var error = Assert.Single(result.Value.Errors);
            Assert.Equal(BulkReviewerErrorType.Conflict, error.ErrorType);
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
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                Reviewer2Set = false,
                ApplicationIds = new List<Guid> { id },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(app);
            _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = false });

            var result = await _handler.Handle(command, default);

            var error = Assert.Single(result.Value.Errors);
            Assert.Equal(BulkReviewerErrorType.MessageFailed, error.ErrorType);
        }

        [Fact]
        public async Task Handle_MultipleApplications_MixedResults()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var app1 = new Data.Entities.Application.Application
            {
                Id = id1,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var app2 = new Data.Entities.Application.Application
            {
                Id = id2,
                Reviewer1 = ReviewerOld1,
                Reviewer2 = ReviewerOld2
            };

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                Reviewer2Set = false,
                ApplicationIds = new List<Guid> { id1, id2 },
                SentByName = SentByName,
                SentByEmail = SentByEmail
            };

            _repository.Setup(r => r.GetByIdAsync(id1)).ReturnsAsync(app1);
            _repository.Setup(r => r.GetByIdAsync(id2)).ReturnsAsync(app2);

            _mediator.SetupSequence(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), default))
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = true })
                     .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse> { Success = false });

            var result = await _handler.Handle(command, default);

            Assert.Equal(2, result.Value.UpdatedCount);
            Assert.Equal(1, result.Value.ErrorCount);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ExceptionHandled()
        {
            var id = Guid.NewGuid();

            var command = new BulkSaveReviewerCommand
            {
                UserType = UserType.Qfau.ToString(),
                Reviewer1Set = true,
                Reviewer1 = ReviewerNew1,
                ApplicationIds = new List<Guid> { id },
                SentByEmail = SentByEmail,
                SentByName = SentByName
            };

            _repository.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("DB error"));

            var result = await _handler.Handle(command, default);

            Assert.False(result.Success);
            Assert.Contains("DB error", result.ErrorMessage);
        }
    }
}
