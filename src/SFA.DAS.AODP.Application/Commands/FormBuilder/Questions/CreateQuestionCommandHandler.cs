using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class CreateQuestionCommandHandler(IQuestionRepository _questionRepository, IPageRepository _pageRepository, IQuestionValidationRepository _questionValidationRepository) : IRequestHandler<CreateQuestionCommand, BaseMediatrResponse<CreateQuestionCommandResponse>>
{
    public async Task<BaseMediatrResponse<CreateQuestionCommandResponse>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateQuestionCommandResponse>();
        try
        {
            if (!await _pageRepository.IsPageEditable(request.PageId)) throw new RecordLockedException();

            var maxOrder = _questionRepository.GetMaxOrderByPageId(request.PageId);

            var questionToCreate = new Entities.FormBuilder.Question()
            {
                Required = request.Required,
                Title = request.Title,
                Order = ++maxOrder,
                PageId = request.PageId,
                Type = request.Type,
            };

            var created = await _questionRepository.Create(questionToCreate);
            await SetQuestionDefaultsAsync(created);

            response.Value.Id = created.Id;
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

    private async Task SetQuestionDefaultsAsync(Entities.FormBuilder.Question question)
    {
        if (question.Type == QuestionType.File.ToString())
        {
            var questionValidation = new QuestionValidation()
            {
                QuestionId = question.Id,
                NumberOfFiles = 1
            };

            await _questionValidationRepository.UpsertAsync(questionValidation);
        }
    }
}
