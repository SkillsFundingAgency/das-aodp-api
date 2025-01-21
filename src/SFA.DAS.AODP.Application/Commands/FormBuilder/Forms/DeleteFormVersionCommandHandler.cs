using MediatR;
using SFA.DAS.AODP.Data.Repositories;

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
            response.Data = success;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}