using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class MoveSectionUpCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<MoveSectionUpCommand, MoveSectionUpCommandResponse>
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;

    public async Task<MoveSectionUpCommandResponse> Handle(MoveSectionUpCommand request, CancellationToken cancellationToken)
    {
        var response = new MoveSectionUpCommandResponse()
        {
        };

        try
        {
            var res = await _sectionRepository.MoveSectionOrderUp(request.SectionId);
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
