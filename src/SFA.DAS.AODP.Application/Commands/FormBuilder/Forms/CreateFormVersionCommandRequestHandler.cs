using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandRequestHandler : IRequestHandler<CreateFormVersionCommandRequest, CreateFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formVersionRepository;
    private readonly IMapper _mapper;

    public CreateFormVersionCommandRequestHandler(IFormVersionRepository formVersionRepository, IMapper mapper)
    {
        _formVersionRepository = formVersionRepository;
        _mapper = mapper;
    }

    public async Task<CreateFormVersionCommandResponse> Handle(CreateFormVersionCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new CreateFormVersionCommandResponse
        {
            Success = false
        };

        try
        {
            var formVersionToCreate = _mapper.Map<Entities.FormVersion>(request.Data);
            var form = _formVersionRepository.Create(formVersionToCreate);
            var createdForm = _mapper.Map<ViewModels.FormVersion>(form);

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
