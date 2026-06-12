using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    public class ValidateFundingExtensionCandidatesCommandHandler
        : IRequestHandler<ValidateFundingExtensionCandidatesCommand, BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>>
    {
        private readonly IRolloverRepository _rolloverRepository;
        private readonly IRolloverFundingExtensionValidator _rolloverFundingExtensionValidator;
        private readonly IFundingExtensionCandidatesCsvBuilder _rolloverWorkflowCandidatesCsvBuilder;

        public ValidateFundingExtensionCandidatesCommandHandler(IRolloverRepository rolloverRepository, IRolloverFundingExtensionValidator rolloverFundingExtensionValidator, IFundingExtensionCandidatesCsvBuilder rolloverWorkflowCandidatesCsvBuilder)
        {
            _rolloverRepository = rolloverRepository;
            _rolloverFundingExtensionValidator = rolloverFundingExtensionValidator;
            _rolloverWorkflowCandidatesCsvBuilder = rolloverWorkflowCandidatesCsvBuilder;
        }

        public async Task<BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>> Handle(ValidateFundingExtensionCandidatesCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>();

            try
            {
                var incomingCandidates = request.FundingExtensionCandidates
                    .Select(x => new CandidateKey(x.Qan, x.FundingStreamName))
                    .ToHashSet();

                var validationContext = await _rolloverRepository.GetFundingExtensionValidationContextAsync(incomingCandidates, cancellationToken);

                var validationResult = _rolloverFundingExtensionValidator.Validate(request.FundingExtensionCandidates, validationContext, cancellationToken);

                byte[]? csvBytes = null;

                if (!validationResult.IsValid)
                {
                    var exportRows = validationResult.Candidates
                        .Select(c => FundingExtensionCandidateExportMapper.Map(c.CandidateDetails))
                        .ToList();

                    csvBytes = _rolloverWorkflowCandidatesCsvBuilder.BuildWithValidationErrors(
                        exportRows,
                        validationResult.Candidates);
                }


                response.Value = new ValidateFundingExtensionCandidatesCommandResponse
                {
                    IsValid = validationResult.IsValid,
                    TotalCandidates = validationResult.TotalCandidates,
                    FailedCandidateCount = validationResult.FailedCandidateCount,
                    FailureSummary = validationResult.FailureSummary,
                    ValidatedCandidateFile = csvBytes
                };


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
    }
}
