using MediatR;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Form;

public class GetAllFormsQueryHandler : IRequestHandler<GetAllFormsQuery, GetAllFormsQueryResponse>
{
    private readonly IFormRepository _formRepository;

    public GetAllFormsQueryHandler(IFormRepository formRepository)
    {
        _formRepository = formRepository;
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

            queryResponse.Data = new();
            foreach (var version in data)
            {
                queryResponse.Data.Add(new()
                {
                    Id = version.FormId,
                    Name = version.Name,
                    Description = version.Description,
                    Version = version.Version,

                });
            }

            queryResponse.Success = true;
        }
        catch (Exception ex)
        {
            queryResponse.ErrorMessage = ex.Message;
        }

        return queryResponse;
    }
}