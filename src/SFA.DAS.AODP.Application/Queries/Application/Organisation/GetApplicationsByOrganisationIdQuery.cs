using MediatR;

public class GetApplicationsByOrganisationIdQuery : IRequest<GetApplicationsByOrganisationIdQueryResponse>
{
    public GetApplicationsByOrganisationIdQuery(Guid organisationId)
    {
        OrganisationId = organisationId;
    }
    public Guid OrganisationId { get; set; }


}
