using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetMatchingQualificationsQuery : IRequest<BaseMediatrResponse<GetMatchingQualificationsQueryResponse>>
    {
        public string SearchTerm { get; set; }
    }
}