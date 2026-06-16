using MediatR;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.FundingExtension;
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
        private readonly IFundingExtensionProjectionService _fundingExtensionProjectionService;

        public ValidateFundingExtensionCandidatesCommandHandler(
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

        public async Task<BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>> Handle(
            ValidateFundingExtensionCandidatesCommand request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>();

            try
            {
                var incomingKeys = request.FundingExtensionCandidates
                    .Select(x => new CandidateKey(x.Qan, x.FundingStreamName))
                    .ToHashSet();

                var validationContext = await _rolloverRepository
                    .GetFundingExtensionValidationContextAsync(incomingKeys, cancellationToken);

                var validationResult = _rolloverFundingExtensionValidator
                    .Validate(request.FundingExtensionCandidates, validationContext, cancellationToken);

                var validationResponse = new ValidateFundingExtensionCandidatesCommandResponse
                {
                    IsValid = validationResult.IsValid
                };

                if (!validationResult.IsValid)
                {
                    var exportRows = validationResult.Candidates
                        .Select(c => FundingExtensionCandidateExportMapper.Map(c.CandidateDetails))
                        .ToList();

                    var csvBytes = _rolloverWorkflowCandidatesCsvBuilder
                        .BuildWithValidationErrors(exportRows, validationResult.Candidates);

                    validationResponse.ValidationFailureSummary = new ValidationFailureSummary
                    {
                        FailedCandidateCount = validationResult.FailedCandidateCount,
                        ValidatedCandidateFile = csvBytes
                    };

                    response.Value = validationResponse;
                    response.Success = true;
                    return response;
                }


                var dbCandidates = await _rolloverRepository
                    .GetFundingExtensionCandidatesAsync(cancellationToken);

                validationResponse.ValidationSuccessSummary = _fundingExtensionProjectionService.ProjectSummary(dbCandidates, request.FundingExtensionCandidates);

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


    }
}
