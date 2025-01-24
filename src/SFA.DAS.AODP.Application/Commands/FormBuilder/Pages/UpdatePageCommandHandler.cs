using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommandHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<UpdatePageCommand, UpdatePageCommandResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<UpdatePageCommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdatePageCommandResponse();
        response.Success = false;

        try
        {
            var pageToUpdate = Mapper.Map<Entities.Page>(request.Data);
            var page = await PageRepository.Update(pageToUpdate);

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
