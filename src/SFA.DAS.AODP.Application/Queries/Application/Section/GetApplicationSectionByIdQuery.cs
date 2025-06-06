﻿using MediatR;using SFA.DAS.AODP.Application;

public class GetApplicationSectionByIdQuery : IRequest<BaseMediatrResponse<GetApplicationSectionByIdQueryResponse>>
{
    public GetApplicationSectionByIdQuery(Guid sectionId, Guid formVersionId)
    {
        SectionId = sectionId;
        FormVersionId = formVersionId;
    }
    public Guid SectionId { get; set; }
    public Guid FormVersionId { get; set; }

}
