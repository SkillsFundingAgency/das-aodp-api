using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class MovePageDownCommandHandler(IPageRepository pageRepository) : IRequestHandler<MovePageDownCommand, MovePageDownCommandResponse>
{
    private readonly IPageRepository _pageRepository = pageRepository;

    public async Task<MovePageDownCommandResponse> Handle(MovePageDownCommand request, CancellationToken cancellationToken)
    {
        var response = new MovePageDownCommandResponse()
        {
        };

        try
        {
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
