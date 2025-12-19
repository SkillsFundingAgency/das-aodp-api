using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Application;
using SFA.DAS.AODP.Models.Form;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, BaseMediatrResponse<CreateApplicationCommandResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IFormVersionRepository _formVersionRepository;
    private readonly IQanValidationService _qanValidationService;

    public CreateApplicationCommandHandler(IApplicationRepository applicationRepository, IFormVersionRepository formVersionRepository, IQanValidationService qanValidationService)
    {
        _applicationRepository = applicationRepository;
        _formVersionRepository = formVersionRepository;
        _qanValidationService = qanValidationService;
    }

    public async Task<BaseMediatrResponse<CreateApplicationCommandResponse>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateApplicationCommandResponse> { Success = false };

        try
        {
            var formVersion = await _formVersionRepository.GetFormVersionByIdAsync(request.FormVersionId);
            if (formVersion.Status != FormVersionStatus.Published.ToString()) throw new InvalidOperationException("The FormVersion is not published");

            if (!string.IsNullOrWhiteSpace(request.QualificationNumber))
            {
                var validation = await _qanValidationService.ValidateAsync(
                    request.QualificationNumber,
                    request.Title,
                    request.OrganisationName,
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

            var form = await _applicationRepository.Create(new()
            {
                FormVersionId = request.FormVersionId,
                Name = request.Title,
                Owner = request.Owner,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrganisationId = request.OrganisationId,
                QualificationNumber = request.QualificationNumber,
                AwardingOrganisationName = request.OrganisationName,
                AwardingOrganisationUkprn = request.OrganisationUkprn,
                Status = ApplicationStatus.Draft.ToString()
            });

            response.Value = new() { Id = form.Id };
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
