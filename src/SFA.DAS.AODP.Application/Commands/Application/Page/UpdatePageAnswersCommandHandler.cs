using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class UpdatePageAnswersCommandHandler : IRequestHandler<UpdatePageAnswersCommand, UpdatePageAnswersCommandResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IApplicationPageRepository _applicationPageRepository;
    private readonly IApplicationQuestionAnswerRepository _questionAnswerRepository;

    public UpdatePageAnswersCommandHandler(IQuestionRepository questionRepository, IApplicationPageRepository applicationPageRepository, IApplicationQuestionAnswerRepository questionAnswerRepository)
    {
        _questionRepository = questionRepository;
        _applicationPageRepository = applicationPageRepository;
        _questionAnswerRepository = questionAnswerRepository;
    }

    public async Task<UpdatePageAnswersCommandResponse> Handle(UpdatePageAnswersCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdatePageAnswersCommandResponse()
        {
            Success = false
        };

        try
        {
            var page = await _applicationPageRepository.GetApplicationPageByPageIdAsync(request.ApplicationId, request.PageId);
            page ??= new()
            {
                PageId = request.PageId,
                ApplicationId = request.ApplicationId,
            };
            page.Status = ApplicationPageStatus.Completed.ToString();

            await _applicationPageRepository.UpsertAsync(page);


            page.QuestionAnswers ??= [];
            foreach (var requestQuestion in request.Questions)
            {
                var answer = page?.QuestionAnswers?.FirstOrDefault(q => q.QuestionId == requestQuestion.QuestionId);

                if (answer == null)
                {
                    answer = new()
                    {
                        QuestionId = requestQuestion.QuestionId,
                        ApplicationPageId = page.Id,
                    };
                    page.QuestionAnswers?.Add(answer);
                }

                if (requestQuestion.QuestionType == QuestionType.Text.ToString())
                {
                    answer.TextValue = requestQuestion?.Answer?.TextValue;
                }
            }
            await _questionAnswerRepository.UpsertAsync(page.QuestionAnswers);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.Success = false;
        }

        return response;
    }
}
