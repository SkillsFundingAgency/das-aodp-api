using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class UpdatePageAnswersCommandHandler : IRequestHandler<UpdatePageAnswersCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IApplicationPageRepository _applicationPageRepository;
    private readonly IApplicationQuestionAnswerRepository _questionAnswerRepository;
    private readonly IPageRepository _pageRepository;
    private readonly IApplicationRepository _applicationRepository;

    public UpdatePageAnswersCommandHandler(IApplicationPageRepository applicationPageRepository, IApplicationQuestionAnswerRepository questionAnswerRepository, IPageRepository pageRepository, IApplicationRepository applicationRepository)
    {
        _applicationPageRepository = applicationPageRepository;
        _questionAnswerRepository = questionAnswerRepository;
        _pageRepository = pageRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdatePageAnswersCommand request, CancellationToken cancellationToken)
    {

        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application.Submitted ?? false) throw new RecordLockedException();

            var formPage = await _pageRepository.GetPageByIdAsync(request.PageId);
            var applicationPage = await _applicationPageRepository.GetApplicationPageByPageIdAsync(request.ApplicationId, request.PageId);
            applicationPage ??= new()
            {
                PageId = request.PageId,
                ApplicationId = request.ApplicationId,
            };
            applicationPage.Status = ApplicationPageStatus.Completed.ToString();

            await _applicationPageRepository.UpsertAsync(applicationPage);


            applicationPage.QuestionAnswers ??= [];
            foreach (var requestQuestion in request.Questions)
            {
                var answer = applicationPage?.QuestionAnswers?.FirstOrDefault(q => q.QuestionId == requestQuestion.QuestionId);

                if (answer == null)
                {
                    answer = new()
                    {
                        QuestionId = requestQuestion.QuestionId,
                        ApplicationPageId = applicationPage.Id,
                    };
                    applicationPage.QuestionAnswers?.Add(answer);
                }

                if (requestQuestion.QuestionType == QuestionType.Text.ToString() || requestQuestion.QuestionType == QuestionType.TextArea.ToString())
                {
                    answer.TextValue = requestQuestion?.Answer?.TextValue;
                }
                else if (requestQuestion.QuestionType == QuestionType.Radio.ToString())
                {
                    answer.OptionsValue = requestQuestion?.Answer?.RadioChoiceValue;
                }
                else if (requestQuestion.QuestionType == QuestionType.MultiChoice.ToString())
                {
                    answer.OptionsValue = string.Join(",", requestQuestion?.Answer?.MultipleChoiceValue ?? []);
                }
                else if (requestQuestion.QuestionType == QuestionType.Number.ToString())
                {
                    answer.NumberValue = requestQuestion?.Answer?.NumberValue;
                }
                else if (requestQuestion.QuestionType == QuestionType.Date.ToString())
                {
                    answer.DateValue = requestQuestion?.Answer?.DateValue;
                }
            }
            await _questionAnswerRepository.UpsertAsync(applicationPage.QuestionAnswers);

            await HandleRoutingAsync(request, formPage);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.Success = false;
        }

        return response;
    }

    private async Task HandleRoutingAsync(UpdatePageAnswersCommand request, Page formPage)
    {
        List<ApplicationPage> applicationPagesToUpsert = new();

        if (request.Routing != null)
        {
            List<Guid> pageIdsToSkip = new();
            // handle pages in same section
            if (request.Routing.NextPageOrder.HasValue || request.Routing.EndSection)
            {
                var currentSectionPagesToSkip = await _pageRepository.GetPagesIdInSectionByOrderAsync(request.SectionId, formPage.Order + 1, request.Routing.NextPageOrder);
                pageIdsToSkip.AddRange(currentSectionPagesToSkip);
            }

            if (request.Routing.NextSectionOrder.HasValue || request.Routing.EndForm)
            {
                var nextSectionsPagesToSkip = await _pageRepository.GetPagesIdInFormBySectionOrderAsync(request.FormVersionId, formPage.Section.Order + 1, request.Routing.NextSectionOrder);
                pageIdsToSkip.AddRange(nextSectionsPagesToSkip);

            }

            var existingApplicationPages = await _applicationPageRepository.GetApplicationPagesByPageIdsAsync(request.ApplicationId, pageIdsToSkip);

            foreach (var pageId in pageIdsToSkip)
            {
                var appPage = existingApplicationPages.FirstOrDefault(f => f.PageId == pageId) ?? new()
                {
                    PageId = pageId,
                    ApplicationId = request.ApplicationId,
                };

                appPage.Status = ApplicationPageStatus.Skipped.ToString();
                appPage.SkippedByQuestionId = request.Routing.QuestionId;

                applicationPagesToUpsert.Add(appPage);
            }

            var existingSkippedPages = await _applicationPageRepository.GetSkippedApplicationPagesByQuestionIdAsync(request.ApplicationId, request.Routing.QuestionId, pageIdsToSkip);
            foreach (var skippedPage in existingSkippedPages)
            {
                skippedPage.Status = ApplicationPageStatus.NotStarted.ToString();
                skippedPage.SkippedByQuestionId = null;
                applicationPagesToUpsert.Add(skippedPage);
            }

        }
        await _applicationPageRepository.UpsertAsync(applicationPagesToUpsert);
    }
}
