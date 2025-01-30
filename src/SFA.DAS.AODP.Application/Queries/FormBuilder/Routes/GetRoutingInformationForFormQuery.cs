using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForFormQuery : IRequest<GetRoutingInformationForFormQueryResponse>
    {
        public Guid FormVersionId { get; set; }
    }
}