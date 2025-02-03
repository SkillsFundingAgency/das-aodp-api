using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<CreateSectionCommand, CreateSectionCommandResponse>
{
    public async Task<CreateSectionCommandResponse> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateSectionCommandResponse();
        try
        {
            var maxOrder = sectionRepository.GetMaxOrderByFormVersionId(request.FormVersionId);
            var createdSection = await sectionRepository.Create(new()
            {
                Description = request.Description,
                Title = request.Title,
                FormVersionId = request.FormVersionId,
                Order = ++maxOrder,
          
            });

            response.Id = createdSection.Id;
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
