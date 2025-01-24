using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Application.Exceptions;

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
        catch (NoForeignKeyException ex)
        {
            response.Success = false;
            response.InnerException = new DependantNotFoundException(ex.ForeignKey);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
