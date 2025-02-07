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

public class MovePageUpCommandHandler(IPageRepository pageRepository) : IRequestHandler<MovePageUpCommand, MovePageUpCommandResponse>
{
    private readonly IPageRepository _pageRepository = pageRepository;

    public async Task<MovePageUpCommandResponse> Handle(MovePageUpCommand request, CancellationToken cancellationToken)
    {
        var response = new MovePageUpCommandResponse()
        {
        };

        try
        {
            var res = await _pageRepository.MovePageOrderUp(request.PageId);
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
