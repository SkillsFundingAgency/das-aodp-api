using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForFormQueryHandler(IRouteRepository _routeRepository, IFormVersionRepository _formVersionRepository)
        : IRequestHandler<GetRoutingInformationForFormQuery, BaseMediatrResponse< GetRoutingInformationForFormQueryResponse>>
    {
        public async Task<BaseMediatrResponse<GetRoutingInformationForFormQueryResponse>> Handle(GetRoutingInformationForFormQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetRoutingInformationForFormQueryResponse>();
            response.Success = false;
            try
            {
                var routes = await _routeRepository.GetQuestionRoutingDetailsByFormVersionId(request.FormVersionId);

                response.Value = GetRoutingInformationForFormQueryResponse.Map(routes, await _formVersionRepository.IsFormVersionEditable(request.FormVersionId));
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