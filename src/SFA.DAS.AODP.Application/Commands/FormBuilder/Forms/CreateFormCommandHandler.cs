using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class CreateFormCommandHandler : IRequestHandler<CreateFormCommand, CreateFormCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;
    private readonly IMapper _mapper;

    public CreateFormCommandHandler(IFormVersionRepository formVersionRepository, IMapper mapper)
    {
        _formVersionRepository = formVersionRepository;
        _mapper = mapper;
    }

    public async Task<CreateFormCommandResponse> Handle(CreateFormCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateFormCommandResponse
        {
            Success = false
        };

        try
        {
            var formVersionToCreate = _mapper.Map<Entities.FormVersion>(request.FormVersion);
            var form = _formVersionRepository.Create(formVersionToCreate);
            var createdForm = _mapper.Map<Models.FormVersion>(form);

            response.FormVersion = createdForm;
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
