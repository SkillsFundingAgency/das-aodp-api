using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreateQuestionCommandHandler(IQuestionRepository _questionRepository) : IRequestHandler<CreateQuestionCommand, CreateQuestionCommandResponse>
{
    public async Task<CreateQuestionCommandResponse> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateQuestionCommandResponse();
        try
        {
            var maxOrder = _questionRepository.GetMaxOrderByPageId(request.PageId);

            var questionToCreate = new Entities.Question()
            {
                Required = request.Required,
                Title = request.Title,
                Order = ++maxOrder,
                PageId = request.PageId,
                Type = request.Type,
            };

            var created = await _questionRepository.Create(questionToCreate);

            response.Id = created.Id;
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
