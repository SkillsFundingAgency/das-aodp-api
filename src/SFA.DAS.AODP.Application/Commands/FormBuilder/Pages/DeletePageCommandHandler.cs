using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommandHandler(IPageRepository _pageRepository) : IRequestHandler<DeletePageCommand, BaseMediatrResponse<EmptyResponse>>
{

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeletePageCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            if (!await _pageRepository.IsPageEditable(request.PageId)) throw new RecordLockedException();

            var res = await _pageRepository.Archive(request.PageId);

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
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
