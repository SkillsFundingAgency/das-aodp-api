using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForFormQueryHandler(IRouteRepository _routeRepository) : IRequestHandler<GetRoutingInformationForFormQuery, GetRoutingInformationForFormQueryResponse>
    {
        public async Task<GetRoutingInformationForFormQueryResponse> Handle(GetRoutingInformationForFormQuery request, CancellationToken cancellationToken)
        {
            var response = new GetRoutingInformationForFormQueryResponse();
            response.Success = false;
            try
            {
                var routes = await _routeRepository.GetQuestionRoutingDetailsByFormVersionId(request.FormVersionId);

                response = GetRoutingInformationForFormQueryResponse.Map(routes);
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