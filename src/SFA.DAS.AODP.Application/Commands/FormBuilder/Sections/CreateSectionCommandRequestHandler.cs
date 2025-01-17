using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandRequestHandler : IRequestHandler<CreateSectionCommandRequest, CreateSectionCommandResponse>
{
    //private readonly IGenericRepository<Section> _sectionRepository;

    public CreateSectionCommandRequestHandler()
    {
    }

    public async Task<CreateSectionCommandResponse> Handle(CreateSectionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new CreateSectionCommandResponse();
        try
        {
            // _sectionRepository.Add(section);

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
