using Markdig;
using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandHandler : IRequestHandler<CreateFormVersionCommand, BaseMediatrResponse<CreateFormVersionCommandResponse>>
{
    private readonly IFormVersionRepository _formVersionRepository;
    private readonly IFormRepository _formRepository;

    public CreateFormVersionCommandHandler(IFormVersionRepository formVersionRepository, IFormRepository formRepository)
    {
        _formVersionRepository = formVersionRepository;
        _formRepository = formRepository;
    }

    public async Task<BaseMediatrResponse<CreateFormVersionCommandResponse>> Handle(CreateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateFormVersionCommandResponse>();

        try
        {
            var order = _formRepository.GetMaxOrder();
            var form = await _formVersionRepository.Create(new()
            {
                Title = request.Title,
                Description = request.Description,
                DescriptionHTML = Markdown.ToHtml(request.Description)
                    .Replace("<a", "<a class=\"govuk-link\""),
            }, order + 1);

            response.Value = new() { Id = form.Id };
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
