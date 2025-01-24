using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryHandler(ISectionRepository sectionRepository) : IRequestHandler<GetAllSectionsQuery, GetAllSectionsQueryResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;


    public async Task<GetAllSectionsQueryResponse> Handle(GetAllSectionsQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAllSectionsQueryResponse();
        response.Success = false;
        try
        {
            var sections = await SectionRepository.GetSectionsForFormAsync(request.FormVersionId);

            response.Data = [.. sections];
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}