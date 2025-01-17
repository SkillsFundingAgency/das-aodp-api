using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQueryRequestHandler : IRequestHandler<GetFormVersionByIdQueryRequest, GetFormVersionByIdQueryResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private IMapper _mapper { get; }

    public GetFormVersionByIdQueryRequestHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<GetFormVersionByIdQueryResponse> Handle(GetFormVersionByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var response = new GetFormVersionByIdQueryResponse();
        response.Success = false;
        try
        {
            var formVersion = await _formRepository.GetFormVersionByIdAsync(request.Id);
            response.Data = _mapper.Map<Entities.FormVersion?, ViewModels.FormVersion?>(formVersion);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
