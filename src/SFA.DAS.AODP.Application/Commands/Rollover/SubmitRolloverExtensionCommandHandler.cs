using MediatR;
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

        public SubmitRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository,
            IQualificationFundingsRepository qualificationFundingsRepository,
            ISubmitFundingExtensionService applyFundingExtensionsService
)
        {
            _rolloverRepository = rolloverRepository;
            _qualificationFundingsRepository = qualificationFundingsRepository;
            _applyFundingExtensionsService = applyFundingExtensionsService;
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

                var fundingKeys = candidates
                    .Select(x => new QualificationFundingKey(
                        x.QualificationVersionId,
                        x.FundingOfferId))
                    .Distinct()
                    .ToList();

                var fundings = await _qualificationFundingsRepository
                    .GetRolloverQualificationFundingsAsync(fundingKeys, cancellationToken);

                var success = await _applyFundingExtensionsService.Submit(candidates, request.Items, fundings, cancellationToken);

                if (!success)
                {
                    response.Success = true;
                    response.Value.ResultMessage = "Failed to apply funding extensions.";
                    return response;
                }

                await _rolloverRepository.SaveChangesAsync(cancellationToken);

                response.Value.ResultMessage = "Funding extensions applied.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.InnerException = ex;
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
