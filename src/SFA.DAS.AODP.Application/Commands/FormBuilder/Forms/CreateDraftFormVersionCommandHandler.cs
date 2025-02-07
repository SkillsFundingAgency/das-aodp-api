using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateDraftFormVersionCommandHandler : IRequestHandler<CreateDraftFormVersionCommand, CreateDraftFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;

    public CreateDraftFormVersionCommandHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;

    }

    public async Task<CreateDraftFormVersionCommandResponse> Handle(CreateDraftFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateDraftFormVersionCommandResponse
        {
            Success = false
        };

        try
        {
            var draft = await _formVersionRepository.GetDraftFormVersionByFormId(request.FormId);
            if (draft != null) throw new InvalidOperationException("A draft version already exists");

            var published = await _formVersionRepository.GetPublishedFormVersionByFormId(request.FormId);
            if (published == null) throw new InvalidOperationException("No published version to clone");

            var newDraft = await _formVersionRepository.CreateDraftAsync(published.Id);

            response.Success = true;
            response.FormVersionId = newDraft.Id;

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}