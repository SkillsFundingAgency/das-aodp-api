using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandHandler : IRequestHandler<UpdateFormVersionCommand, UpdateFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private readonly IMapper _mapper;

    public UpdateFormVersionCommandHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<UpdateFormVersionCommandResponse> Handle(UpdateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var formVersionToUpdate = _mapper.Map<Entities.FormVersion>(request.Data);
            var form = _formRepository.Update(formVersionToUpdate);
            var updatedForm = _mapper.Map<UpdateFormVersionCommandResponse.FormVersion>(form);

            response.Data = updatedForm;
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
