using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class PublishFormVersionCommandHandler : IRequestHandler<PublishFormVersionCommand, PublishFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;

    public PublishFormVersionCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<PublishFormVersionCommandResponse> Handle(PublishFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new PublishFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var found = await _formRepository.Publish(request.FormVersionId);

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
