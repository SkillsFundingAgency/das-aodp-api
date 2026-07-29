using MediatR;
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
        private readonly IFundingChangeCoordinator _fundingChangeCoordinator;
        private readonly IRolloverFundingEligibilityRepository _fundingEligibilityRepository;

        public SubmitRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository,
            IRolloverFundingUpdateRepository rolloverFundingUpdateRepository,
            ISubmitFundingExtensionService applyFundingExtensionsService,
            IFundingChangeCoordinator fundingChangeCoordinator,
            IRolloverFundingEligibilityRepository fundingEligibilityRepository)
        {
            _rolloverRepository = rolloverRepository;
            _rolloverFundingUpdateRepository = rolloverFundingUpdateRepository;
            _applyFundingExtensionsService = applyFundingExtensionsService;
            _fundingChangeCoordinator = fundingChangeCoordinator;
            _fundingEligibilityRepository = fundingEligibilityRepository;
        }

        public async Task<BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>> Handle(
            SubmitRolloverExtensionCommand request, 
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<SubmitRolloverExtensionCommandResponse>();

            try
            {
                var keys = request.Items
                    .Select(x => new CandidateKey
                    (
                        x.Qan!,
                        x.FundingStreamName!
                    ))
                    .ToList();

                var candidates = await _rolloverRepository
                    .LoadRolloverCandidateGraphAsync(keys, cancellationToken);

                if (candidates.Count == 0)
                {
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

                var fundingUpdates = await _rolloverFundingUpdateRepository
                    .GetFundingUpdatesAsync(fundingKeys, cancellationToken);

                await _fundingChangeCoordinator.ExecuteAsync(
                    changeSet,
                    async ct =>
                    {
                        var eligibility = await _fundingEligibilityRepository.GetAsync(
                            changeSet.Keys,
                            ct);

                        if (eligibility.Count != changeSet.Keys.Count ||
                            eligibility.Any(x => !x.IsEligible))
                        {
                            throw new InvalidOperationException(
                                "One or more rollover candidates are no longer backed by applicable funding.");
                        }

                        var success = await _applyFundingExtensionsService.Submit(
                            candidates,
                            request.Items,
                            fundingUpdates,
                            ct);

                        if (!success)
                        {
                            throw new FundingExtensionApplicationException(
                                "Failed to apply funding extensions.");
                        }

                        return true;
                    },
                    cancellationToken);

                response.Value.ResultMessage = "Funding extensions applied.";
                response.Success = true;
            }
            catch (FundingExtensionApplicationException ex)
            {
                response.Success = true;
                response.Value.ResultMessage = ex.Message;
            }
            catch (Exception ex)
            {
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
