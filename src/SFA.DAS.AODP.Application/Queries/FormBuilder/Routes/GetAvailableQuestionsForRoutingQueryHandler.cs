using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableQuestionsForRoutingQueryHandler(IRouteRepository _routeRepository)
        : IRequestHandler<GetAvailableQuestionsForRoutingQuery, BaseMediatrResponse<GetAvailableQuestionsForRoutingQueryResponse>>
    {
        public async Task<BaseMediatrResponse<GetAvailableQuestionsForRoutingQueryResponse>> Handle(GetAvailableQuestionsForRoutingQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetAvailableQuestionsForRoutingQueryResponse>();
            response.Success = false;
            try
            {
                var question = await _routeRepository.GetAvailableQuestionsForRoutingByPageId(request.PageId);

                response.Value = question;
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
