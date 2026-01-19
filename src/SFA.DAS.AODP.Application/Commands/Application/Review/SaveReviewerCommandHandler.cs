using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveReviewerCommandHandler : IRequestHandler<SaveReviewerCommand, BaseMediatrResponse<SaveReviewerCommandResponse>>
    {
        private readonly IApplicationRepository _repository;
        private readonly IMediator _mediator;

        public SaveReviewerCommandHandler(IApplicationRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<SaveReviewerCommandResponse>> Handle(SaveReviewerCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<SaveReviewerCommandResponse>();

            try
            {
                if (!Enum.TryParse(request.UserType, true, out UserType userType))
                {
                    throw new ArgumentException($"Invalid User Type: {request.UserType}");
                }

                var application = await _repository.GetByIdAsync(request.ApplicationId);

                var newReviewer = request.ReviewerValue?.Trim();

                string? previousReviewer;
                string? otherReviewer;

                switch (request.ReviewerFieldName)
                {
                    case nameof(application.Reviewer1):
                        previousReviewer = application.Reviewer1?.Trim();
                        otherReviewer = application.Reviewer2?.Trim();
                        application.Reviewer1 = newReviewer; 
                        break;

                    case nameof(application.Reviewer2):
                        previousReviewer = application.Reviewer2?.Trim();
                        otherReviewer = application.Reviewer1?.Trim();
                        application.Reviewer2 = newReviewer; 
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.ReviewerFieldName), request.ReviewerFieldName, "Unknown reviewer field");
                }

                if (!string.IsNullOrWhiteSpace(newReviewer) &&
                    !string.IsNullOrWhiteSpace(otherReviewer) &&
                    string.Equals(newReviewer, otherReviewer, StringComparison.OrdinalIgnoreCase))
                {
                    response.Success = true;
                    response.Value.DuplicateReviewerError = true;
                    return response;
                }

                await _repository.UpdateAsync(application);

                var msgCommand = new CreateApplicationMessageCommand()
                {
                    ApplicationId = request.ApplicationId,
                    MessageText = $"Previous {request.ReviewerFieldName}: {previousReviewer}\nNew {request.ReviewerFieldName}: {request.ReviewerValue}",
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                    UserType = userType.ToString(),
                    MessageType = MessageType.QfauOwnerUpdated.ToString()
                };

                var msgResult = await _mediator.Send(msgCommand);
                if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
                response.Success = false;
            }
            return response;
        }
    }
}