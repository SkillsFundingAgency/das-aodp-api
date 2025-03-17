using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunsByNameQuery : IRequest<BaseMediatrResponse<GetJobRunsByNameQueryResponse>>
    {
        public string JobName { get; }

        public GetJobRunsByNameQuery(string name)
        {
            JobName = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
        }
    }
}
