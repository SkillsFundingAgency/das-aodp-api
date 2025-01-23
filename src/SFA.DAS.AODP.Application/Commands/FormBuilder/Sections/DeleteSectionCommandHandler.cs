using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommandHandler(ISectionRepository sectionRepository, IMapper mapper) : IRequestHandler<DeleteSectionCommand, DeleteSectionCommandResponse>
{
    private readonly ISectionRepository SectionRepository = sectionRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<DeleteSectionCommandResponse> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteSectionCommandResponse();

        try
        {
            var res = await SectionRepository.DeleteSection(request.SectionId);

            if (res is null)
            {
                response.Success = false;
                response.ErrorMessage = $"Section with id '{request.SectionId}' could not be found.";
                return response;
            }
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
