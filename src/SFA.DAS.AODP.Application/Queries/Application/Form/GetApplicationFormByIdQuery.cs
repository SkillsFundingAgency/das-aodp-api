using MediatR;

public class GetApplicationFormByIdQuery : IRequest<GetApplicationFormByIdQueryResponse>
{
    public GetApplicationFormByIdQuery(Guid formVersionId)
    {
        FormVersionId = formVersionId;
    }
    public Guid FormVersionId { get; set; }

}
