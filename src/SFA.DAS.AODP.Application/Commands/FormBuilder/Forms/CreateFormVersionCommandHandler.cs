using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandHandler : IRequestHandler<CreateFormVersionCommand, CreateFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;
    private readonly IMapper _mapper;

    public CreateFormVersionCommandHandler(IFormVersionRepository formVersionRepository, IMapper mapper)
    {
        _formVersionRepository = formVersionRepository;
        _mapper = mapper;
    }

    public async Task<CreateFormVersionCommandResponse> Handle(CreateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateFormVersionCommandResponse
        {
            Success = false
        };

        try
        {
            var formVersionToCreate = _mapper.Map<Entities.FormVersion>(request.Data);
            var form = await _formVersionRepository.Create(formVersionToCreate);
            var createdForm = _mapper.Map<CreateFormVersionCommandResponse.FormVersion>(form);

            response.Data = createdForm;
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
