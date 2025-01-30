using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQueryHandler(ISectionRepository _sectionRepository, IPageRepository _pageRepository, IQuestionRepository _questionRepository) : IRequestHandler<GetRoutingInformationForQuestionQuery, GetRoutingInformationForQuestionQueryResponse>
    {
        public async Task<GetRoutingInformationForQuestionQueryResponse> Handle(GetRoutingInformationForQuestionQuery request, CancellationToken cancellationToken)
        {
            var response = new GetRoutingInformationForQuestionQueryResponse();
            response.Success = false;
            try
            {
                var question = await _questionRepository.GetQuestionDetailForRoutingAsync(request.QuestionId);

                var nextPages = await _pageRepository.GetNextPagesInSectionByOrderAsync(question.Page.SectionId, question.Page.Order);
                var nextSections = await _sectionRepository.GetNextSectionsByOrderAsync(request.FormVersionId, question.Page.Section.Order);

                response = GetRoutingInformationForQuestionQueryResponse.Map(question, nextSections, nextPages);
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