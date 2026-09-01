using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    public class SubmitRolloverExtensionCommandHandler
        : IRequestHandler<SubmitRolloverExtensionCommand, BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>>
    {
        private readonly IRolloverRepository _rolloverRepository;
        private readonly IRolloverFundingUpdateRepository _rolloverFundingUpdateRepository;
        private readonly ISubmitFundingExtensionService _applyFundingExtensionsService;
        private readonly IRolloverFundingEligibilityRepository _fundingEligibilityRepository;
        private readonly ILogger<SubmitRolloverExtensionCommandHandler> _logger;

        public SubmitRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository,
            IRolloverFundingUpdateRepository rolloverFundingUpdateRepository,
            ISubmitFundingExtensionService applyFundingExtensionsService,
            IRolloverFundingEligibilityRepository fundingEligibilityRepository,
            ILogger<SubmitRolloverExtensionCommandHandler> logger)
        {

            _rolloverRepository = rolloverRepository;
            _rolloverFundingUpdateRepository = rolloverFundingUpdateRepository;
            _applyFundingExtensionsService = applyFundingExtensionsService;
            _fundingEligibilityRepository = fundingEligibilityRepository;
            _logger = logger;
        }

        public async Task<BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>> Handle(
            SubmitRolloverExtensionCommand request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>();
            var totalStarted = Stopwatch.GetTimestamp();

            try
            {
                var keys = request.Items
                    .Select(x => new CandidateKey
                    (
                        x.Qan!,
                        x.FundingStreamName!
                    ))
                    .ToList();

                var candidateLoadStarted = Stopwatch.GetTimestamp();
                var candidates = await _rolloverRepository.LoadRolloverCandidateGraphAsync(keys, cancellationToken);

                _logger.LogInformation(
                    "Loaded {CandidateCount} rollover candidates for {RequestedItemCount} submitted items in {ElapsedMilliseconds} ms",
                    candidates.Count,
                    request.Items.Count,
                    Stopwatch.GetElapsedTime(candidateLoadStarted).TotalMilliseconds);

                if (candidates.Count == 0)
                {
                    _logger.LogInformation(
                        "Completed funding-extension submission with no matching candidates in {ElapsedMilliseconds} ms",
                        Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);

                    response.Success = true;
                    response.Value.ResultMessage = "No matching rollover candidates were found.";
                    return response;
                }

                var changeSet = FundingChangeSet.Create(candidates.Select(x =>
                    new FundingChangeKey(
                        x.SourceType,
                        x.SourceQualificationId,
                        x.FundingOfferId,
                        x.AcademicYear)));

                var fundingKeys = changeSet.Keys
                    .Select(x => new SourceQualificationFundingKey(
                        x.SourceType,
                        x.SourceQualificationId,
                        x.FundingOfferId,
                        x.AcademicYear!))
                    .ToList();

                var fundingLoadStarted = Stopwatch.GetTimestamp();
                var fundingUpdates = await _rolloverFundingUpdateRepository
                    .GetFundingUpdatesAsync(fundingKeys, cancellationToken);

                _logger.LogInformation(
                    "Loaded {FundingCount} funding update records in {ElapsedMilliseconds} ms",
                    fundingUpdates.Count,
                    Stopwatch.GetElapsedTime(fundingLoadStarted).TotalMilliseconds);

                var eligibility = await _fundingEligibilityRepository.GetAsync(
                    changeSet.Keys,
                    cancellationToken);

                if (eligibility.Count != changeSet.Keys.Count ||
                    eligibility.Any(x => !x.IsEligible))
                {
                    throw new InvalidOperationException(
                        "One or more rollover candidates are no longer backed by applicable funding.");
                }

                var submissionStarted = Stopwatch.GetTimestamp();
                var success = await _applyFundingExtensionsService.Submit(
                    candidates,
                    request.Items,
                    fundingUpdates,
                    cancellationToken);

                _logger.LogInformation(
                    "Applied and persisted funding-extension changes with success status {SubmissionSuccess} in {ElapsedMilliseconds} ms",
                    success,
                    Stopwatch.GetElapsedTime(submissionStarted).TotalMilliseconds);

                if (!success)
                {
                    _logger.LogWarning(
                        "Funding-extension submission failed after {ElapsedMilliseconds} ms for {RequestedItemCount} submitted items",
                        Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds,
                        request.Items.Count);

                    throw new FundingExtensionApplicationException(
                        "Failed to apply funding extensions.");
                }

                response.Value.ResultMessage = "Funding extensions applied.";
                response.Success = true;

                _logger.LogInformation(
                    "Completed funding-extension submission for {RequestedItemCount} submitted items in {ElapsedMilliseconds} ms",
                    request.Items.Count,
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
            }
            catch (FundingExtensionApplicationException ex)
            {
                response.Success = true;
                response.Value.ResultMessage = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Funding-extension submission threw an exception after {ElapsedMilliseconds} ms for {RequestedItemCount} submitted items",
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds,
                    request.Items.Count);

                response.InnerException = ex;
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        private sealed class FundingExtensionApplicationException(string message)
            : Exception(message);
    }
}
