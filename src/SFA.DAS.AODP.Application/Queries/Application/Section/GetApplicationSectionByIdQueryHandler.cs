using MediatR;using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationSectionByIdQueryHandler : IRequestHandler<GetApplicationSectionByIdQuery, BaseMediatrResponse< GetApplicationSectionByIdQueryResponse>>
{
    private readonly ISectionRepository _sectionRepository;

    public GetApplicationSectionByIdQueryHandler(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<BaseMediatrResponse<GetApplicationSectionByIdQueryResponse>> Handle(GetApplicationSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationSectionByIdQueryResponse>();
        try
        {
            var result = await _sectionRepository.GetSectionByIdAsync(request.SectionId);
            response.Value = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
