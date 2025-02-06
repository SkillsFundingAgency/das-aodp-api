﻿using MediatR;

public class GetApplicationFormStatusByApplicationIdQuery : IRequest<GetApplicationFormStatusByApplicationIdQueryResponse>
{
    public GetApplicationFormStatusByApplicationIdQuery(Guid formVersionId, Guid applicationId)
    {
        FormVersionId = formVersionId;
        ApplicationId = applicationId;
    }
    public Guid FormVersionId { get; set; }
    public Guid ApplicationId { get; set; }

}
