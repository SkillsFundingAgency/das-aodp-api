using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryHandler(ISectionRepository _sectionRepository, IFormVersionRepository _formVersionRepository, IRouteRepository _routeRepository)
    : IRequestHandler<GetSectionByIdQuery, BaseMediatrResponse<GetSectionByIdQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetSectionByIdQueryResponse>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetSectionByIdQueryResponse>
        {
            Success = false
        };
        var hasAssociatedRoutes = false;

        try
        {
            var section = await _sectionRepository.GetSectionByIdWithPagesAndQestionsAsync(request.SectionId);

            // Check routes for Sections Directly
            var routesForSection = await _routeRepository.GetRoutesBySectionId(request.SectionId);
            if (routesForSection != null && routesForSection.Any())
            {
                hasAssociatedRoutes = true;
            }

            // Check routes for related Pages and their Questions
            if (!hasAssociatedRoutes) 
            {
                foreach (var page in section.Pages)
                {
                    var routesForPage = await _routeRepository.GetRoutesByPageId(page.Id);
                    if (routesForPage?.Any() == true)
                    {
                        hasAssociatedRoutes = true;
                        break;
                    }

                    foreach (var question in page.Questions)
                    {
                        var routesForQuestion = await _routeRepository.GetRoutesByQuestionId(question.Id);
                        if (routesForQuestion?.Any() == true)
                        {
                            hasAssociatedRoutes = true;
                            break;
                        }
                    }

                    if (hasAssociatedRoutes) break;
                }
            }

            response.Value = section;
            response.Value.HasAssociatedRoutes = hasAssociatedRoutes;
            response.Value.Editable = await _formVersionRepository.IsFormVersionEditable(request.FormVersionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
