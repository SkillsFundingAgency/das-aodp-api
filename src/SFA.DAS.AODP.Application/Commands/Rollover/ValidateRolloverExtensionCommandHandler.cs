using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;
using System.Diagnostics;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    public class ValidateRolloverExtensionCommandHandler
        : IRequestHandler<ValidateRolloverExtensionCommand, BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>>
    {
        private readonly IRolloverRepository _rolloverRepository;
        private readonly IRolloverFundingExtensionValidator _rolloverFundingExtensionValidator;
        private readonly IFundingExtensionCandidatesCsvBuilder _rolloverWorkflowCandidatesCsvBuilder;
        private readonly IFundingExtensionProjectionService _fundingExtensionProjectionService;
        private readonly ILogger<ValidateRolloverExtensionCommandHandler> _logger;

        public ValidateRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository, 
            IRolloverFundingExtensionValidator rolloverFundingExtensionValidator, 
            IFundingExtensionCandidatesCsvBuilder rolloverWorkflowCandidatesCsvBuilder,
            IFundingExtensionProjectionService fundingExtensionProjectionService,
            ILogger<ValidateRolloverExtensionCommandHandler> logger)
        {
            _rolloverRepository = rolloverRepository;
            _rolloverFundingExtensionValidator = rolloverFundingExtensionValidator;
            _rolloverWorkflowCandidatesCsvBuilder = rolloverWorkflowCandidatesCsvBuilder;
            _fundingExtensionProjectionService = fundingExtensionProjectionService;
            _logger = logger;
        }

        public async Task<BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>> Handle(
            ValidateRolloverExtensionCommand request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>();
            var totalStarted = Stopwatch.GetTimestamp();
            var candidateCount = request.RolloverCandidates?.Count ?? 0;

            try
            {
                if (request.RolloverCandidates == null || request.RolloverCandidates.Count == 0)
                {
                    _logger.LogInformation(
                        "Completed rollover candidate validation with no candidates in {ElapsedMilliseconds} ms",
                        Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
                    return GeneralFailureResponse("No rollover candidates were provided for validation.");
                }

                var incomingKeys = request.RolloverCandidates
                    .Select(x => new CandidateKey(x.Qan, x.FundingStreamName))
                    .ToHashSet();

                var contextLoadStarted = Stopwatch.GetTimestamp();
                var validationContext = await _rolloverRepository
                    .GetFundingExtensionValidationContextAsync(incomingKeys, cancellationToken);
                _logger.LogInformation(
                    "Loaded rollover validation context for {CandidateCount} candidates in {ElapsedMilliseconds} ms",
                    candidateCount,
                    Stopwatch.GetElapsedTime(contextLoadStarted).TotalMilliseconds);

                var validationStarted = Stopwatch.GetTimestamp();
                var validationResult = _rolloverFundingExtensionValidator
                    .Validate(request.RolloverCandidates, validationContext, cancellationToken);
                _logger.LogInformation(
                    "Validated {CandidateCount} rollover candidates with {FailedCandidateCount} failures in {ElapsedMilliseconds} ms",
                    candidateCount,
                    validationResult.FailedCandidateCount,
                    Stopwatch.GetElapsedTime(validationStarted).TotalMilliseconds);

                var validationResponse = new ValidateRolloverExtensionCommandResponse
                {
                    IsValid = validationResult.IsValid
                };

                if (!validationResult.IsValid)
                {
                    var workflowRunLoadStarted = Stopwatch.GetTimestamp();
                    var latestRunId = await _rolloverRepository.GetLatestWorkflowRunIdAsync(cancellationToken);
                    _logger.LogInformation(
                        "Loaded the latest rollover workflow run in {ElapsedMilliseconds} ms",
                        Stopwatch.GetElapsedTime(workflowRunLoadStarted).TotalMilliseconds);

                    if (latestRunId == Guid.Empty)
                        throw new InvalidOperationException("No workflow runs exist");

                    var exportLoadStarted = Stopwatch.GetTimestamp();
                    var exportRows = await _rolloverRepository.GetRolloverWorkflowCandidatesByRunId(latestRunId.Value, cancellationToken);
                    _logger.LogInformation(
                        "Loaded {ExportRowCount} rollover export rows in {ElapsedMilliseconds} ms",
                        exportRows.Count,
                        Stopwatch.GetElapsedTime(exportLoadStarted).TotalMilliseconds);

                    var failureResponseStarted = Stopwatch.GetTimestamp();
                    var exportLookup = exportRows.ToDictionary(
                        x => new CandidateKey(x.QAN, x.FundingStreamName));

                    var missingCandidates = new List<CandidateNotIncludedInRollover>();

                    foreach (var result in validationResult.Candidates)
                    {
                        var uploaded = result.CandidateDetails;
                       
                        var key = new CandidateKey(uploaded.Qan.OrEmpty(), uploaded.FundingStreamName);

                        if (exportLookup.TryGetValue(key, out var exportRow))
                        {
                            RolloverCandidateExportMapper.ApplyUserUpdates(exportRow, uploaded);
                        }
                        else
                        {
                            missingCandidates.Add(new CandidateNotIncludedInRollover(result));
                        }
                    }

                    var csvBytes = _rolloverWorkflowCandidatesCsvBuilder
                        .BuildWithValidationErrors(exportRows, validationResult.Candidates);

                    validationResponse.ValidationFailureSummary = new ValidationFailureSummary
                    {
                        FailedCandidateCount = validationResult.FailedCandidateCount,
                        ValidatedCandidateFile = csvBytes,
                        NotIncludedInRollover = missingCandidates

                    };

                    response.Value = validationResponse;
                    response.Success = true;
                    _logger.LogInformation(
                        "Built rollover validation failure response with {MissingCandidateCount} missing candidates in {ElapsedMilliseconds} ms; total validation time was {TotalElapsedMilliseconds} ms",
                        missingCandidates.Count,
                        Stopwatch.GetElapsedTime(failureResponseStarted).TotalMilliseconds,
                        Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
                    return response;
                }

                var statusLoadStarted = Stopwatch.GetTimestamp();
                var dbCandidatesStatus = await _rolloverRepository.GetRolloverCandidatesStatusAsync(cancellationToken);
                _logger.LogInformation(
                    "Loaded {CandidateStatusCount} rollover candidate statuses in {ElapsedMilliseconds} ms",
                    dbCandidatesStatus.Count,
                    Stopwatch.GetElapsedTime(statusLoadStarted).TotalMilliseconds);

                var projectionStarted = Stopwatch.GetTimestamp();
                validationResponse.ValidationSuccessSummary = _fundingExtensionProjectionService.ProjectSummary(dbCandidatesStatus, request.RolloverCandidates);
                _logger.LogInformation(
                    "Projected rollover validation summary in {ElapsedMilliseconds} ms",
                    Stopwatch.GetElapsedTime(projectionStarted).TotalMilliseconds);

                response.Value = validationResponse;
                response.Success = true;
                _logger.LogInformation(
                    "Completed rollover candidate validation for {CandidateCount} candidates in {ElapsedMilliseconds} ms",
                    candidateCount,
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
            }
            catch (RecordLockedException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Rollover candidate validation encountered a locked record after {ElapsedMilliseconds} ms",
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
                response.Success = false;
                response.InnerException = new LockedRecordException();
            }
            catch (NoForeignKeyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Rollover candidate validation encountered a missing dependency after {ElapsedMilliseconds} ms",
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
                response.Success = false;
                response.InnerException = new DependantNotFoundException(ex.ForeignKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Rollover candidate validation failed after {ElapsedMilliseconds} ms for {CandidateCount} candidates",
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds,
                    candidateCount);
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
            }

            return response;
        }

        private BaseMediatrResponse<ValidateRolloverExtensionCommandResponse> GeneralFailureResponse(string message)
        {
            return new BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>
            {
                Success = true,
                Value = new ValidateRolloverExtensionCommandResponse
                {
                    IsValid = false,
                    ValidationFailureSummary = new ValidationFailureSummary
                    {
                        GeneralFailureMessage = message,
                    }
                }
            };
        }

    }
}
