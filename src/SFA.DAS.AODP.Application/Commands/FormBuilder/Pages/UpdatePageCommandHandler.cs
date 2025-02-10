using AutoMapper;
using MediatR;using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommandHandler(IPageRepository _pageRepository) : IRequestHandler<UpdatePageCommand, BaseMediatrResponse<EmptyResponse>>
{  

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            if (!await _pageRepository.IsPageEditable(request.Id)) throw new RecordLockedException();

            var section = await _pageRepository.GetPageByIdAsync(request.Id);
            section.Title = request.Title;


            await _pageRepository.Update(section);
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
