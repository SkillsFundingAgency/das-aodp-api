using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationSectionByIdQueryHandler : IRequestHandler<GetApplicationSectionByIdQuery, GetApplicationSectionByIdQueryResponse>
{
    private readonly ISectionRepository _sectionRepository;

    public GetApplicationSectionByIdQueryHandler(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<GetApplicationSectionByIdQueryResponse> Handle(GetApplicationSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationSectionByIdQueryResponse();
        response.Success = false;
        try
        {
            var result = await _sectionRepository.GetSectionByIdAsync(request.SectionId);
            response = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
