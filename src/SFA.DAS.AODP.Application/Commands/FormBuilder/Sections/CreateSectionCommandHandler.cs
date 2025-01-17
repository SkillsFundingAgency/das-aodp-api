using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, CreateSectionCommandResponse>
{
    //private readonly IGenericRepository<Section> _sectionRepository;

    public CreateSectionCommandHandler()
    {
    }

    public async Task<CreateSectionCommandResponse> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateSectionCommandResponse();
        try
        {
            var section = new Section
            {
                FormId = request.FormId,
                Order = request.Order,
                Title = request.Title,
                Description = request.Description,
                NextSectionId = request.NextSectionId,
            };

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
