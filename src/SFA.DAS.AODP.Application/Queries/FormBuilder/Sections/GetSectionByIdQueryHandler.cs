using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<GetSectionByIdQuery, GetSectionByIdQueryResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetSectionByIdQueryResponse> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSectionByIdQueryResponse(new());
        response.Success = false;
        try
        {
            var section = await SectionRepository.GetSectionByIdAsync(request.SectionId);

            if (section is null)
            {
                response.Success = false;
                response.ErrorMessage = $"Section with id '{request.SectionId}' could not be found.";
                return response;
            }

            response.Data = Mapper.Map<GetSectionByIdQueryResponse.Section>(section);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
