using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IFormRepository _formRepository;

    public DeleteFormCommandHandler(IFormRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            await _formRepository.Archive(request.FormId);
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