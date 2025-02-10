using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandHandler : IRequestHandler<CreateFormVersionCommand, BaseMediatrResponse<CreateFormVersionCommandResponse>>
{
    private readonly IFormVersionRepository _formVersionRepository;


    public CreateFormVersionCommandHandler(IFormVersionRepository formVersionRepository)
    {
        _formVersionRepository = formVersionRepository;

    }

    public async Task<BaseMediatrResponse<CreateFormVersionCommandResponse>> Handle(CreateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateFormVersionCommandResponse>();

        try
        {
            var order = _formVersionRepository.GetMaxOrder();
            var form = await _formVersionRepository.Create(new()
            {
                Title = request.Title,
                Description = request.Description,
                Order = order + 1,
            });

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
