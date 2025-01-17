using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;


namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class GetFormByVersionIdQueryHandler : IRequestHandler<GetFormByVersionIdQuery, GetFormByVersionIdQueryResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private IMapper _mapper { get; }

    public GetFormByVersionIdQueryHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }


    public async Task<GetFormByVersionIdQueryResponse> Handle(GetFormByVersionIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetFormByVersionIdQueryResponse();
        response.Success = false;
        try
        {
            var formVersion = await _formRepository.GetFormVersionByIdAsync(request.Id);
            response.Data = _mapper.Map<Entities.FormVersion?, Models.FormVersion?>(formVersion);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
