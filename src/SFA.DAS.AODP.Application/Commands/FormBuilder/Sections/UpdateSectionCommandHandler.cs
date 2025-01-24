using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommandHandler(ISectionRepository _sectionRepository) : IRequestHandler<UpdateSectionCommand, UpdateSectionCommandResponse>
{

    public async Task<UpdateSectionCommandResponse> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateSectionCommandResponse();
        response.Success = false;

        try
        {
            var section = await _sectionRepository.GetSectionByIdAsync(request.Id);
            section.Title = request.Title;
            section.Description = request.Description;


            await _sectionRepository.Update(section);
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
