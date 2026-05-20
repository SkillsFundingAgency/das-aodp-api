using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class BulkSaveReviewerCommandHandler
        : IRequestHandler<BulkSaveReviewerCommand, BaseMediatrResponse<BulkSaveReviewerCommandResponse>>
    {
        private readonly IApplicationRepository _repository;
        private readonly IMediator _mediator;

        public BulkSaveReviewerCommandHandler(IApplicationRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<BulkSaveReviewerCommandResponse>> Handle(
            BulkSaveReviewerCommand request,
            CancellationToken cancellationToken)
        {
            var result = new BaseMediatrResponse<BulkSaveReviewerCommandResponse>();

            try
            {
                var userType = ParseUserType(request.UserType);

                var errors = new List<BulkReviewerErrorDto>();
                var updatedCount = 0;

                foreach (var applicationReviewId in request.ApplicationReviewIds)
                {
                    var updated = await ProcessApplicationAsync(
                        applicationReviewId,
                        request,
                        userType,
                        errors,
                        cancellationToken);

                    if (updated)
                    {
                        updatedCount++;
                    }
                }

                result.Value = new BulkSaveReviewerCommandResponse
                {
                    RequestedCount = request.ApplicationReviewIds.Count,
                    UpdatedCount = updatedCount,
                    ErrorCount = errors.Count,
                    Errors = errors
                };

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.InnerException = ex;
            }

            return result;
        }

        private static UserType ParseUserType(string userType)
        {
            if (!Enum.TryParse(userType, true, out UserType parsedUserType))
            {
                throw new ArgumentException($"Invalid User Type: {userType}");
            }

            return parsedUserType;
        }

        private async Task<bool> ProcessApplicationAsync(
            Guid applicationReviewId,
            BulkSaveReviewerCommand request,
            UserType userType,
            List<BulkReviewerErrorDto> errors,
            CancellationToken cancellationToken)
        {
            Data.Entities.Application.Application? application = null;

            try
            {
                application = await _repository.GetByReviewIdAsync(applicationReviewId);
            }
            catch (RecordNotFoundException)
            {
                errors.Add(new BulkReviewerErrorDto
                {
                    ApplicationId = applicationReviewId,
                    Title = "Application not found",
                    ErrorType = BulkReviewerErrorType.Missing
                });

                return false;
            }

            var oldReviewer1 = application.Reviewer1;
            var oldReviewer2 = application.Reviewer2;

            var newReviewer1 = request.Reviewer1Set ? request.Reviewer1 : oldReviewer1;
            var newReviewer2 = request.Reviewer2Set ? request.Reviewer2 : oldReviewer2;

            bool reviewer1Changed = request.Reviewer1Set && oldReviewer1 != newReviewer1;
            bool reviewer2Changed = request.Reviewer2Set && oldReviewer2 != newReviewer2;

            if (!reviewer1Changed && !reviewer2Changed)
            {
                return false;
            }

            if (ReviewerAssignmentRules.WouldCauseConflict(newReviewer1, newReviewer2))
            {
                errors.Add(CreateError(application, BulkReviewerErrorType.Conflict));
                return false;
            }

            application.Reviewer1 = newReviewer1;
            application.Reviewer2 = newReviewer2;

            await _repository.UpdateAsync(application);

            var changes = new List<string>();

            if (oldReviewer1 != newReviewer1)
            {
                changes.Add($"Previous Reviewer1: {oldReviewer1}\nNew Reviewer1: {newReviewer1}");
            }

            if (oldReviewer2 != newReviewer2)
            {
                changes.Add($"Previous Reviewer2: {oldReviewer2}\nNew Reviewer2: {newReviewer2}");
            }

            var msgCommand = new CreateApplicationMessageCommand
            {
                ApplicationId = application.Id,
                MessageText = string.Join("\n", changes),
                SentByEmail = request.SentByEmail,
                SentByName = request.SentByName,
                UserType = userType.ToString(),
                MessageType = MessageType.QfauOwnerUpdated.ToString()
            };

            var msgResult = await _mediator.Send(msgCommand, cancellationToken);

            if (!msgResult.Success)
            {
                errors.Add(CreateError(application, BulkReviewerErrorType.MessageFailed));
            }

            return true;
        }

        private static BulkReviewerErrorDto CreateError(Data.Entities.Application.Application application, BulkReviewerErrorType errorType)
        {
            return new BulkReviewerErrorDto
            {
                ApplicationId = application.Id,
                ReferenceNumber = application.ReferenceId,
                Qan = application.QualificationNumber,
                Title = application.Name,
                AwardingOrganisation = application.AwardingOrganisationName,
                ErrorType = errorType
            };
        }
    }
}
