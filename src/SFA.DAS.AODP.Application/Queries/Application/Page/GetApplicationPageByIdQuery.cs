using MediatR;

public class GetApplicationPageByIdQuery : IRequest<GetApplicationPageByIdQueryResponse>
{
    public GetApplicationPageByIdQuery(int pageOrder, Guid sectionId, Guid formVersionId)
    {
        PageOrder = pageOrder;
        SectionId = sectionId;
        FormVersionId = formVersionId;
    }

    public int PageOrder { get; set; }
    public Guid SectionId { get; set; }
    public Guid FormVersionId { get; set; }

}
