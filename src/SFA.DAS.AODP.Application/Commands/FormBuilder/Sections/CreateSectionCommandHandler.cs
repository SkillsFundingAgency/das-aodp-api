using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandHandler(ISectionRepository _sectionRepository, IFormVersionRepository _formVersionRepository)
    : IRequestHandler<CreateSectionCommand, BaseMediatrResponse<CreateSectionCommandResponse>>
{
    public async Task<BaseMediatrResponse<CreateSectionCommandResponse>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateSectionCommandResponse>();
        try
        {
            if (!await _formVersionRepository.IsFormVersionEditable(request.FormVersionId)) throw new RecordLockedException();

            var maxOrder = _sectionRepository.GetMaxOrderByFormVersionId(request.FormVersionId);
            var createdSection = await _sectionRepository.Create(new()
            {
                Title = request.Title,
                FormVersionId = request.FormVersionId,
                Order = ++maxOrder,

            });

            response.Value = new() { Id = createdSection.Id };
            response.Success = true;
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
