using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<GetAllSectionsQuery, GetAllSectionsQueryResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetAllSectionsQueryResponse> Handle(GetAllSectionsQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAllSectionsQueryResponse(new());
        response.Success = false;
        try
        {
            var sections = await SectionRepository.GetSectionsForFormAsync(request.FormVersionId);

            response.Data = Mapper.Map<List<GetAllSectionsQueryResponse.Section>>(sections);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}