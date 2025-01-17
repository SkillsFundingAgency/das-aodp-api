using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommandRequestHandler : IRequestHandler<DeleteFormVersionCommandRequest, DeleteFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public DeleteFormVersionCommandRequestHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;
    }

    public async Task<DeleteFormVersionCommandResponse> Handle(DeleteFormVersionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new DeleteFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var success = await _formVersionRepository.Archive(request.Id);
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