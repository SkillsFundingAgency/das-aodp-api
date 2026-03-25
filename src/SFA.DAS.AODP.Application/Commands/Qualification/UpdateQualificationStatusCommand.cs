using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Qualification;

[ExcludeFromCodeCoverage]
public class UpdateQualificationStatusCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public string QualificationReference { get; set; } = string.Empty;
    public Guid ProcessStatusId { get; set; }
    public int? Version { get; set; }
    public string? UserDisplayName { get; set; }
    public string? Notes { get; set; }
}
