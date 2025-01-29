using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableSectionsAndPagesForRoutingQueryHandler(IRouteRepository _routeRepository) : IRequestHandler<GetAvailableSectionsAndPagesForRoutingQuery,GetAvailableSectionsAndPagesForRoutingQueryResponse>
    {
        public async Task<GetAvailableSectionsAndPagesForRoutingQueryResponse> Handle(GetAvailableSectionsAndPagesForRoutingQuery request, CancellationToken cancellationToken)
        {
            var response = new GetAvailableSectionsAndPagesForRoutingQueryResponse();
            response.Success = false;
            try
            {
                var question = await _routeRepository.GetAvailableSectionsAndPagesForRoutingByFormVersionId(request.FormVersionId);

                response = question;
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
