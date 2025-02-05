using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationDetailsQuery : IRequest<GetQualificationDetailsQueryResponse>
    {
        public int Id { get; set; }
    }
}
