namespace SFA.DAS.AODP.Models.Rollover;

public sealed record RolloverQueryBuilderRequest
{
    public IReadOnlyCollection<int> LevelIds { get; init; } = [];
    public IReadOnlyCollection<int> TypeIds { get; init; } = [];
    public IReadOnlyCollection<string> SectorSubjectAreaIds { get; init; } = [];
    public IReadOnlyCollection<Guid> AwardingOrganisationIds { get; init; } = [];
}
