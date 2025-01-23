using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommandHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<DeletePageCommand, DeletePageCommandResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<DeletePageCommandResponse> Handle(DeletePageCommand request, CancellationToken cancellationToken)
    {
        var response = new DeletePageCommandResponse();

        try
        {
            var res = await PageRepository.Archive(request.PageId);

            if (res is null)
            {
                response.Success = false;
                response.ErrorMessage = $"Page with id '{request.PageId}' could not be found.";
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
