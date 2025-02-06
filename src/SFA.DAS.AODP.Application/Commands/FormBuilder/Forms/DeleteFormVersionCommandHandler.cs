using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommandHandler : IRequestHandler<DeleteFormVersionCommand, DeleteFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public DeleteFormVersionCommandHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<DeleteFormVersionCommandResponse> Handle(DeleteFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var success = await _formVersionRepository.Archive(request.FormVersionId);
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