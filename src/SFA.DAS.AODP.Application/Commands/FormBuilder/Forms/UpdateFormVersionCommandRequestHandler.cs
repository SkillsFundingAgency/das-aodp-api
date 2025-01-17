using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandRequestHandler : IRequestHandler<UpdateFormVersionCommandRequest, UpdateFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private readonly IMapper _mapper;

    public UpdateFormVersionCommandRequestHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<UpdateFormVersionCommandResponse> Handle(UpdateFormVersionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new UpdateFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var formVersionToUpdate = _mapper.Map<Entities.FormVersion>(request.Data);
            var form = _formRepository.Update(formVersionToUpdate);
            var updatedForm = _mapper.Map<ViewModels.FormVersion>(form);

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
