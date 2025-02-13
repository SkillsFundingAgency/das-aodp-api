using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Form;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, BaseMediatrResponse<CreateApplicationCommandResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IFormVersionRepository _formVersionRepository;

    public CreateApplicationCommandHandler(IApplicationRepository applicationRepository, IFormVersionRepository formVersionRepository)
    {
        _applicationRepository = applicationRepository;
        _formVersionRepository = formVersionRepository;
    }

    public async Task<BaseMediatrResponse<CreateApplicationCommandResponse>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateApplicationCommandResponse>();

        try
        {
            var formVersion = await _formVersionRepository.GetFormVersionByIdAsync(request.FormVersionId);
            if (formVersion.Status != FormVersionStatus.Published.ToString()) throw new InvalidOperationException("The FormVersion is not published");

            var form = await _applicationRepository.Create(new()
            {
                FormVersionId = request.FormVersionId,
                Name = request.Title,
                Owner = request.Owner,
                CreatedAt = DateTime.UtcNow,
            });

            response.Value = new() { Id = form.Id };
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
