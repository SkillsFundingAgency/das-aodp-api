using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

[ExcludeFromCodeCoverage]
public record GetRolloverWorkflowCandidatesCountQuery() : IRequest<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>;
