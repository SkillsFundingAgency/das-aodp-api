using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class MovePageDownCommandHandler(IPageRepository pageRepository) : IRequestHandler<MovePageDownCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IPageRepository _pageRepository = pageRepository;

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MovePageDownCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>()
        {
        };

        try
        {
            if (!await _pageRepository.IsPageEditable(request.PageId)) throw new RecordLockedException();

            var res = await _pageRepository.MovePageOrderDown(request.PageId);
            response.Success = true;
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
