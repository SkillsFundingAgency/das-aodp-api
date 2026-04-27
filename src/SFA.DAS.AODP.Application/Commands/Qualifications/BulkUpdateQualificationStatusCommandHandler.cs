using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class BulkUpdateQualificationStatusCommandHandler
        : IRequestHandler<BulkUpdateQualificationStatusCommand, BaseMediatrResponse<BulkUpdateQualificationStatusResponse>>
    {
        private const string MissingTitle = "Qualification not found";
        private const string StatusUpdateFailedTitle = "Failed to update status";
        private const string HistoryFailedTitle = "Status updated but failed to add history";
        private const string InvalidBulkUpdateStatus = "Status '{0}' cannot be applied via bulk update.";

        private static readonly HashSet<string> AllowedBulkStatuses = new()
        {
            Data.Enum.ProcessStatus.OnHold,
            Data.Enum.ProcessStatus.NoActionRequired,
            Data.Enum.ProcessStatus.DecisionRequired
        };

        private readonly IQualificationsRepository _qualificationsRepository;

        public BulkUpdateQualificationStatusCommandHandler(IQualificationsRepository qualificationsRepository)
        {
            _qualificationsRepository = qualificationsRepository;
        }

        public async Task<BaseMediatrResponse<BulkUpdateQualificationStatusResponse>> Handle(
            BulkUpdateQualificationStatusCommand request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<BulkUpdateQualificationStatusResponse>();

            try
            {

                var status = await _qualificationsRepository.GetProcessStatusOrThrow(request.ProcessStatusId);
                if (!AllowedBulkStatuses.Contains(status.Name))
                {
                    response.Success = false;
                    response.ErrorMessage = string.Format(InvalidBulkUpdateStatus, status.Name);
                    return response;
                }

                var result = await _qualificationsRepository.BulkUpdateQualificationStatusWithHistoryAsync(
                    qualificationIds: request.QualificationIds,
                    processStatusId: request.ProcessStatusId,
                    userDisplayName: request.UserDisplayName,
                    comment: request.Comment,
                    cancellationToken: cancellationToken);

                var requestedDistinctIds = request.QualificationIds
                    .Where(x => x != Guid.Empty)
                    .Distinct()
                    .ToList();

                var errors = new List<BulkQualificationErrorDto>();

                errors.AddRange(result.MissingIds.Select(id => new BulkQualificationErrorDto
                {
                    QualificationId = id,
                    Qan = string.Empty,
                    Title = MissingTitle,
                    ErrorType = BulkQualificationErrorType.Missing
                }));

                errors.AddRange(result.StatusUpdateFailed.Select(f => new BulkQualificationErrorDto
                {
                    QualificationId = f.QualificationId,
                    Qan = f.Qan,
                    Title = StatusUpdateFailedTitle,
                    ErrorType = BulkQualificationErrorType.StatusUpdateFailed
                }));

                errors.AddRange(result.HistoryFailed.Select(f => new BulkQualificationErrorDto
                {
                    QualificationId = f.QualificationId,
                    Qan = f.Qan,
                    Title = HistoryFailedTitle,
                    ErrorType = BulkQualificationErrorType.HistoryFailed
                }));

                if (result.Status is null) { throw new InvalidOperationException("Repository returned null status unexpectedly."); }

                response.Value = new BulkUpdateQualificationStatusResponse
                {
                    ProcessStatusId = result.Status.Id,
                    ProcessStatusName = result.Status.Name,

                    RequestedCount = requestedDistinctIds.Count,
                    UpdatedCount = result.Succeeded.Count,
                    ErrorCount = errors.Count,

                    Errors = errors
                };

                response.Success = true;
            }
            catch (NoForeignKeyException ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = new DependantNotFoundException(request.ProcessStatusId);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
            }

            return response;
        }
    }
}