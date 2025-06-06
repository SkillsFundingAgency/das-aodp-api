﻿using MediatR;using SFA.DAS.AODP.Application;

public class GetApplicationsByOrganisationIdQuery : IRequest<BaseMediatrResponse<GetApplicationsByOrganisationIdQueryResponse>>
{
    public GetApplicationsByOrganisationIdQuery(Guid organisationId)
    {
        OrganisationId = organisationId;
    }
    public Guid OrganisationId { get; set; }


}
