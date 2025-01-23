using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;

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

            if (!found)
            {
                response.ErrorMessage = $"Not found form version with ID {request.FormVersionId}. ";
                response.NotFound = true;
                response.Success = false;
            }
            else
            {
                response.NotFound = false;
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
