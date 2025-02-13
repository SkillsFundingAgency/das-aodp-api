using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommandHandler(IPageRepository _pageRepository, ISectionRepository _sectionRepository) : IRequestHandler<CreatePageCommand, BaseMediatrResponse<CreatePageCommandResponse>>
{
    public async Task<BaseMediatrResponse<CreatePageCommandResponse>> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreatePageCommandResponse>();
        try
        {
            if (!await _sectionRepository.IsSectionEditable(request.SectionId)) throw new RecordLockedException();

            var maxOrder = _pageRepository.GetMaxOrderBySectionId(request.SectionId);

            var pageToCreate = new Entities.FormBuilder.Page()
            {
                Title = request.Title,
                Order = ++maxOrder,
                SectionId = request.SectionId

            };

            var createdPage = await _pageRepository.Create(pageToCreate);

            response.Value = new() { Id = createdPage.Id };
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
