using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommandHandler(IPageRepository pageRepository) : IRequestHandler<UpdatePageCommand, UpdatePageCommandResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    

    public async Task<UpdatePageCommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdatePageCommandResponse();
        response.Success = false;

        try
        {
            var section = await PageRepository.GetPageByIdAsync(request.Id);
            section.Title = request.Title;
            section.Description = request.Description;


            await PageRepository.Update(section);
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
