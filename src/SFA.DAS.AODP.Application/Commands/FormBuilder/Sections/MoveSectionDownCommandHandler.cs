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

public class MoveSectionDownCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<MoveSectionDownCommand, MoveSectionDownCommandResponse>
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;

    public async Task<MoveSectionDownCommandResponse> Handle(MoveSectionDownCommand request, CancellationToken cancellationToken)
    {
        var response = new MoveSectionDownCommandResponse()
        {
        };

        try
        {
            var res = await _sectionRepository.MoveSectionOrderDown(request.SectionId);
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
