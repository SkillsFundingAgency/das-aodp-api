using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommandHandler(ISectionRepository _sectionRepository) : IRequestHandler<UpdateSectionCommand, BaseMediatrResponse<EmptyResponse>>
{

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        response.Success = false;

        try
        {
            if (!await _sectionRepository.IsSectionEditable(request.Id)) throw new RecordLockedException();

            var section = await _sectionRepository.GetSectionByIdAsync(request.Id);
            section.Title = request.Title;


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
