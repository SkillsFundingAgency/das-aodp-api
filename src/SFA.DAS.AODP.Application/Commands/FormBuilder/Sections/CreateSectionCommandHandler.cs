using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<CreateSectionCommand, CreateSectionCommandResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<CreateSectionCommandResponse> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateSectionCommandResponse();
        try
        {
            var sectionToCreate = Mapper.Map<Entities.Section>(request.Data);
            var createdSection = await SectionRepository.Create(sectionToCreate);


            response.Data = Mapper.Map<CreateSectionCommandResponse.Section>(createdSection);
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
