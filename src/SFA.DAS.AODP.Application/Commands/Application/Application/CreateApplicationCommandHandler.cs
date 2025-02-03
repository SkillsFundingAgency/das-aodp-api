using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, CreateApplicationCommandResponse>
{
    private readonly IApplicationRepository _applicationRepository;

    public CreateApplicationCommandHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<CreateApplicationCommandResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateApplicationCommandResponse
        {
            Success = false
        };

        try
        {
            var form = await _applicationRepository.Create(new()
            {
                FormVersionId = request.FormVersionId,
                Name = request.Title,
                Owner = request.Owner,
            });

            response.Id = form.Id;
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
