using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;

public class EditApplicationCommandHandler : IRequestHandler<EditApplicationCommand, BaseMediatrResponse<EditApplicationCommandResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IQanValidationService _qanValidationService;

    public EditApplicationCommandHandler(IApplicationRepository applicationRepository, IQanValidationService qanValidationService)
    {
        _applicationRepository = applicationRepository;
        _qanValidationService = qanValidationService;
    }

    public async Task<BaseMediatrResponse<EditApplicationCommandResponse>> Handle(EditApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EditApplicationCommandResponse> { Success = false };

        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application.Submitted == true) throw new RecordLockedException();

            var qanChanged = !string.Equals(
                application.QualificationNumber?.Trim(),
                request.QualificationNumber?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (qanChanged && !string.IsNullOrWhiteSpace(request.QualificationNumber))
            {
                var validation = await _qanValidationService.ValidateAsync(
                    request.QualificationNumber,
                    request.Title,
                    application.AwardingOrganisationName,
                    cancellationToken);

                if (!validation.IsValid)
                {
                    response.Value = new()
                    {
                        IsQanValid = false,
                        QanValidationMessage = validation.ValidationMessage ?? "Invalid QAN"
                    };
                    response.Success = true;
                    return response;
                }
            }

            application.Name = request.Title;
            application.QualificationNumber = request.QualificationNumber;
            application.Owner = request.Owner;

            await _applicationRepository.UpdateAsync(application);
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