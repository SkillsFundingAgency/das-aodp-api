﻿using MediatR;

public class GetApplicationSectionStatusByApplicationIdQuery : IRequest<GetApplicationSectionStatusByApplicationIdQueryResponse>
{
    public GetApplicationSectionStatusByApplicationIdQuery(Guid sectionId, Guid formVersionId, Guid applicationId)
    {
        SectionId = sectionId;
        FormVersionId = formVersionId;
        ApplicationId = applicationId;
    }
    public Guid SectionId { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid ApplicationId { get; set; }

}
