using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    public class SubmitRolloverExtensionCommandHandler
        : IRequestHandler<SubmitRolloverExtensionCommand, BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>>
    {
        private readonly IRolloverRepository _rolloverRepository;
        private readonly IQualificationFundingsRepository _qualificationFundingsRepository;
        private readonly ISubmitFundingExtensionService _applyFundingExtensionsService;
        private readonly ILogger<SubmitRolloverExtensionCommandHandler> _logger;

        public SubmitRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository,
            IQualificationFundingsRepository qualificationFundingsRepository,
            ISubmitFundingExtensionService applyFundingExtensionsService,
            ILogger<SubmitRolloverExtensionCommandHandler> logger)
        {
            _rolloverRepository = rolloverRepository;
            _qualificationFundingsRepository = qualificationFundingsRepository;
            _applyFundingExtensionsService = applyFundingExtensionsService;
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
                var candidates = await _rolloverRepository
                    .LoadRolloverCandidateGraphAsync(keys, cancellationToken);
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

                var fundingKeys = candidates
                    .Select(x => new QualificationFundingKey(
                        x.QualificationVersionId,
                        x.FundingOfferId))
                    .Distinct()
                    .ToList();

                var fundingLoadStarted = Stopwatch.GetTimestamp();
                var fundings = await _qualificationFundingsRepository
                    .GetRolloverQualificationFundingsAsync(fundingKeys, cancellationToken);
                _logger.LogInformation(
                    "Loaded {FundingCount} qualification funding records in {ElapsedMilliseconds} ms",
                    fundings.Count,
                    Stopwatch.GetElapsedTime(fundingLoadStarted).TotalMilliseconds);

                var submissionStarted = Stopwatch.GetTimestamp();
                var success = await _applyFundingExtensionsService.Submit(candidates, request.Items, fundings, cancellationToken);
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
                    response.Success = true;
                    response.Value.ResultMessage = "Failed to apply funding extensions.";
                    return response;
                }

                response.Value.ResultMessage = "Funding extensions applied.";
                response.Success = true;
                _logger.LogInformation(
                    "Completed funding-extension submission for {RequestedItemCount} submitted items in {ElapsedMilliseconds} ms",
                    request.Items.Count,
                    Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
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
    }
}
