using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommandHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<UpdateSectionCommand, UpdateSectionCommandResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<UpdateSectionCommandResponse> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateSectionCommandResponse();
        response.Success = false;

        try
        {
            var sectionToUpdate = Mapper.Map<Entities.Section>(request.Data);
            var section = await SectionRepository.Update(sectionToUpdate);

            if (section == null)
            {
                response.Success = false;
                response.ErrorMessage = $"Section with id '{request.Data.Id}' could not be found.";
                return response;
            }
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
