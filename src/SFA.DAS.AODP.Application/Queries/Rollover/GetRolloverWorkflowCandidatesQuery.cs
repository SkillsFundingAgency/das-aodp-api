using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public record GetRolloverWorkflowCandidatesQuery(int? Skip = 0, int? Take = 0) : IRequest<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>;
