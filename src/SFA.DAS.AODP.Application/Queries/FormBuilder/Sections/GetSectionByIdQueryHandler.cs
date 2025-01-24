using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<GetSectionByIdQuery, GetSectionByIdQueryResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetSectionByIdQueryResponse> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSectionByIdQueryResponse();
        response.Success = false;
        try
        {
            var section = await SectionRepository.GetSectionByIdAsync(request.SectionId);

            response.Data = Mapper.Map<GetSectionByIdQueryResponse.Section>(section);
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
