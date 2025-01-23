using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommandHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<UpdatePageCommand, UpdatePageCommandResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<UpdatePageCommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdatePageCommandResponse();
        response.Success = false;

        try
        {
            var pageToUpdate = Mapper.Map<Entities.Page>(request.Data);
            var page = await PageRepository.Update(pageToUpdate);

            if (page == null)
            {
                response.Success = false;
                response.ErrorMessage = $"Page with id '{request.Data.Id}' could not be found.";
                return response;
            }
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
