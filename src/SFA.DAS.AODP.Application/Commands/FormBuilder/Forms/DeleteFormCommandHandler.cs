using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, DeleteFormCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public DeleteFormCommandHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<DeleteFormCommandResponse> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteFormCommandResponse();
        response.Success = false;

        try
        {
            var sucessful = await _formVersionRepository.Archive(request.Id);
            if (!sucessful)
            {
                response.Success = false;
                response.ErrorMessage = $"Not found form version for ID '{request.Id}'";
            }
            else
            {
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}