using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetNewQualificationsQueryHandler : IRequestHandler<GetNewQualificationsQuery, GetNewQualificationsQueryResponse>
    {
        private readonly INewQualificationsRepository _repository;

        public GetNewQualificationsQueryHandler(INewQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetNewQualificationsQueryResponse> Handle(GetNewQualificationsQuery request, CancellationToken cancellationToken)
        {
            var qualifications = await _repository.GetAllNewQualificationsAsync();
            return new GetNewQualificationsQueryResponse { Success = true, NewQualifications = qualifications };
        }
    }
}
