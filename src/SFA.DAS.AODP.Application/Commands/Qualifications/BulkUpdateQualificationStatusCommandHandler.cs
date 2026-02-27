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

                var errors = new List<BulkQualificationErrorDTO>();

                errors.AddRange(result.MissingIds.Select(id => new BulkQualificationErrorDTO
                {
                    QualificationId = id,
                    Qan = string.Empty,
                    Title = MissingTitle,
                    ErrorType = BulkQualificationErrorType.Missing
                }));

                errors.AddRange(result.StatusUpdateFailed.Select(f => new BulkQualificationErrorDTO
                {
                    QualificationId = f.QualificationId,
                    Qan = f.Qan,
                    Title = StatusUpdateFailedTitle,
                    ErrorType = BulkQualificationErrorType.StatusUpdateFailed
                }));

                errors.AddRange(result.HistoryFailed.Select(f => new BulkQualificationErrorDTO
                {
                    QualificationId = f.QualificationId,
                    Qan = f.Qan,
                    Title = HistoryFailedTitle,
                    ErrorType = BulkQualificationErrorType.HistoryFailed
                }));

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