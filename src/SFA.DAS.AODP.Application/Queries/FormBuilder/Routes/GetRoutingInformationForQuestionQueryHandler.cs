using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQueryHandler(ISectionRepository _sectionRepository, IPageRepository _pageRepository, IQuestionRepository _questionRepository, IFormVersionRepository _formVersionRepository)
        : IRequestHandler<GetRoutingInformationForQuestionQuery, BaseMediatrResponse<GetRoutingInformationForQuestionQueryResponse>>
    {
        public async Task<BaseMediatrResponse<GetRoutingInformationForQuestionQueryResponse>> Handle(GetRoutingInformationForQuestionQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetRoutingInformationForQuestionQueryResponse>();
            try
            {
                var question = await _questionRepository.GetQuestionDetailForRoutingAsync(request.QuestionId);

                var nextPages = await _pageRepository.GetNextPagesInSectionByOrderAsync(question.Page.SectionId, question.Page.Order);
                var nextSections = await _sectionRepository.GetNextSectionsByOrderAsync(request.FormVersionId, question.Page.Section.Order);

                response.Value = GetRoutingInformationForQuestionQueryResponse.Map(question, nextSections, nextPages, await _formVersionRepository.IsFormVersionEditable(request.FormVersionId));
                response.Success = true;
            }
            catch (RecordNotFoundException ex)
            {
                response.Success = false;
                response.InnerException = new NotFoundException(ex.Id);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}