using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Application.Exceptions;

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
            response.Success = true;
        }
        catch (RecordLockedException)
        {
            response.Success = false;
            response.InnerException = new LockedRecordException();
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
