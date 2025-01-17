using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, UpdateFormCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private readonly IMapper _mapper;

    public UpdateFormCommandHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<UpdateFormCommandResponse> Handle(UpdateFormCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateFormCommandResponse();
        response.Success = false;

        try
        {
            var formVersionToUpdate = _mapper.Map<Entities.FormVersion>(request.FormVersion);
            var form = _formRepository.Update(formVersionToUpdate);
            var updatedForm = _mapper.Map<Models.FormVersion>(form);

            response.FormVersion = updatedForm;
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
