using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommandHandler(IPageRepository _pageRepository) : IRequestHandler<CreatePageCommand, CreatePageCommandResponse>
{


    public async Task<CreatePageCommandResponse> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new CreatePageCommandResponse();
        try
        {
            var maxOrder = _pageRepository.GetMaxOrderBySectionId(request.SectionId);

            var pageToCreate = new Entities.Page()
            {
                Description = request.Description,
                Title = request.Title,
                Order = ++maxOrder,
                SectionId = request.SectionId

            };

            var createdPage = await _pageRepository.Create(pageToCreate);

            response.Id = createdPage.Id;
            response.Success = true;
        }
        catch (RecordLockedException)
        {
            response.Success = false;
            response.InnerException = new LockedRecordException();
        }
        catch (NoForeignKeyException ex)
        {
            response.Success = false;
            response.InnerException = new DependantNotFoundException(ex.ForeignKey);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
