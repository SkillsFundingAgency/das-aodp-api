﻿using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationOutputFileQuery: IRequest<BaseMediatrResponse<GetQualificationOutputFileResponse>>
    {
        public string CurrentUsername { get; set; } = string.Empty;

        public GetQualificationOutputFileQuery(string username)
        { 
            CurrentUsername = username;
        }
    }
}
