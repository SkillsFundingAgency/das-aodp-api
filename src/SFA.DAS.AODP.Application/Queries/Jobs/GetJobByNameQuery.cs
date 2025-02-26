using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobByNameQuery : IRequest<BaseMediatrResponse<GetJobByNameQueryResponse>>
    {
        public string Name { get; }

        public GetJobByNameQuery(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
        }
    }

}
