using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryHandler(ISectionRepository _sectionRepository, IFormVersionRepository _formVersionRepository)
    : IRequestHandler<GetSectionByIdQuery, BaseMediatrResponse<GetSectionByIdQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetSectionByIdQueryResponse>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetSectionByIdQueryResponse>
        {
            Success = false
        };

        try
        {
            var section = await _sectionRepository.GetSectionByIdAsync(request.SectionId);

            response.Value = section;
            response.Value.HasAssociatedRoutes = await _sectionRepository.HasRoutesForSectionAsync(request.SectionId);
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
