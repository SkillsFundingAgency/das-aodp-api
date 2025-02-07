﻿using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPageByIdQuery : IRequest<GetPageByIdQueryResponse>
{
    public readonly Guid PageId;
    public readonly Guid SectionId;
    public readonly Guid FormVersionId;

    public GetPageByIdQuery(Guid pageId, Guid sectionId, Guid formVersionId)
    {
        PageId = pageId;
        SectionId = sectionId;
        FormVersionId = formVersionId;
    }
}
