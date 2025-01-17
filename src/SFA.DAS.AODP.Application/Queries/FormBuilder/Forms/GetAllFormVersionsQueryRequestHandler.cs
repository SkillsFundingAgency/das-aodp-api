using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryRequestHandler : IRequestHandler<GetAllFormVersionsQueryRequest, GetAllFormVersionsQueryResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private readonly IMapper _mapper;

    public GetAllFormVersionsQueryRequestHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<GetAllFormVersionsQueryResponse> Handle(GetAllFormVersionsQueryRequest request, CancellationToken cancellationToken)
    {
        var queryResponse = new GetAllFormVersionsQueryResponse
        {
            Success = false
        };
        try
        {
            var data = await _formRepository.GetLatestFormVersions();

            queryResponse.Data = _mapper.Map<List<Entities.FormVersion>, List<ViewModels.FormVersion>>(data);
            queryResponse.Success = true;
        }
        catch (Exception ex)
        {
            queryResponse.ErrorMessage = ex.Message;
        }

        return queryResponse;
    }
}