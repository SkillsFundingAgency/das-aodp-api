using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UnpublishFormVersionCommandHandler : IRequestHandler<UnpublishFormVersionCommand, UnpublishFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;

    public UnpublishFormVersionCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<UnpublishFormVersionCommandResponse> Handle(UnpublishFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new UnpublishFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var found = await _formRepository.Unpublish(request.FormVersionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
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
