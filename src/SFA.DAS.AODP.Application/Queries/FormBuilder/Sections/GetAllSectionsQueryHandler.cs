using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryHandler(ISectionRepository sectionRepository) : IRequestHandler<GetAllSectionsQuery, BaseMediatrResponse< GetAllSectionsQueryResponse>>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;


    public async Task<BaseMediatrResponse<GetAllSectionsQueryResponse>> Handle(GetAllSectionsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetAllSectionsQueryResponse>();
        try
        {
            var sections = await SectionRepository.GetSectionsForFormAsync(request.FormVersionId);

            response.Value.Data = [.. sections];
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}