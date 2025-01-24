using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<DeleteSectionCommand, DeleteSectionCommandResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;

    public async Task<DeleteSectionCommandResponse> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteSectionCommandResponse();

        try
        {
            var res = await SectionRepository.DeleteSection(request.SectionId);
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
