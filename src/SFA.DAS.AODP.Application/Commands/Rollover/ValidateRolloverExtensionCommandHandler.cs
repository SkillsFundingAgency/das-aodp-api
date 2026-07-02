using MediatR;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;
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

        public ValidateRolloverExtensionCommandHandler(
            IRolloverRepository rolloverRepository, 
            IRolloverFundingExtensionValidator rolloverFundingExtensionValidator, 
            IFundingExtensionCandidatesCsvBuilder rolloverWorkflowCandidatesCsvBuilder,
            IFundingExtensionProjectionService fundingExtensionProjectionService)
        {
            _rolloverRepository = rolloverRepository;
            _rolloverFundingExtensionValidator = rolloverFundingExtensionValidator;
            _rolloverWorkflowCandidatesCsvBuilder = rolloverWorkflowCandidatesCsvBuilder;
            _fundingExtensionProjectionService = fundingExtensionProjectionService;
        }

        public async Task<BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>> Handle(
            ValidateRolloverExtensionCommand request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>();

            try
            {
                if (request.RolloverCandidates == null || request.RolloverCandidates.Count == 0)
                {
                    return GeneralFailureResponse("No rollover candidates were provided for validation.");
                }

                var incomingKeys = request.RolloverCandidates
                    .Select(x => new CandidateKey(x.Qan, x.FundingStreamName))
                    .ToHashSet();

                var validationContext = await _rolloverRepository
                    .GetFundingExtensionValidationContextAsync(incomingKeys, cancellationToken);

                var validationResult = _rolloverFundingExtensionValidator
                    .Validate(request.RolloverCandidates, validationContext, cancellationToken);

                var validationResponse = new ValidateRolloverExtensionCommandResponse
                {
                    IsValid = validationResult.IsValid
                };

                if (!validationResult.IsValid)
                {
                    var latestRunId = await _rolloverRepository.GetLatestWorkflowRunIdAsync(cancellationToken);

                    if (latestRunId == Guid.Empty)
                        throw new InvalidOperationException("No workflow runs exist");

                    var exportRows = await _rolloverRepository.GetRolloverWorkflowCandidatesByRunId(latestRunId.Value, cancellationToken);

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
                    return response;
                }

                var dbCandidatesStatus = await _rolloverRepository.GetRolloverCandidatesStatusAsync(cancellationToken);

                validationResponse.ValidationSuccessSummary = _fundingExtensionProjectionService.ProjectSummary(dbCandidatesStatus, request.RolloverCandidates);

                response.Value = validationResponse;
                response.Success = true;
            }
            catch (RecordLockedException)
            {
                response.Success = false;
                response.InnerException = new LockedRecordException();
            }
            catch (NoForeignKeyException ex)
            {
                response.Success = false;
                response.InnerException = new DependantNotFoundException(ex.ForeignKey);
            }
            catch (Exception ex)
            {
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
