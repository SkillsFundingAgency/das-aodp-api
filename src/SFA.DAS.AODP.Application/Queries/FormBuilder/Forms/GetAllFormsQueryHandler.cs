using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Form;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class GetAllFormsQueryHandler : IRequestHandler<GetAllFormsQuery, GetAllFormsQueryResponse>
{
    private readonly IFormVersionRepository _formRepository;
    private readonly IMapper _mapper;

    public GetAllFormsQueryHandler(IFormVersionRepository formRepository, IMapper mapper)
    {
        _formRepository = formRepository;
        _mapper = mapper;
    }

    public async Task<GetAllFormsQueryResponse> Handle(GetAllFormsQuery request, CancellationToken cancellationToken)
    {
        var queryResponse = new GetAllFormsQueryResponse
        {
            Success = false
        };
        try
        {
            var data = await _formRepository.GetLatestFormVersions();

            queryResponse.Data = _mapper.Map<List<Entities.FormVersion>, List<Models.FormVersion>>(data);
            queryResponse.Success = true;
        }
        catch (Exception ex)
        {
            queryResponse.ErrorMessage = ex.Message;
        }

        return queryResponse;
    }
}