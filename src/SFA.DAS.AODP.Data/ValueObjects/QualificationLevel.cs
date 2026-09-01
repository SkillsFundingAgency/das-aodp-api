using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.ValueObjects;

[ExcludeFromCodeCoverage]
public record QualificationLevel
{
    public static readonly QualificationLevel EntryLevel = new(0, "Entry level");
    public static readonly QualificationLevel Level1 = new(1, "Level 1");
    public static readonly QualificationLevel Level1Or2 = new(12, "Level 1/Level 2");
    public static readonly QualificationLevel Level2 = new(2, "Level 2");
    public static readonly QualificationLevel Level3 = new(3, "Level 3");
    public static readonly QualificationLevel Level4 = new(4, "Level 4");
    public static readonly QualificationLevel Level5 = new(5, "Level 5");
    public static readonly QualificationLevel Level6 = new(6, "Level 6");
    public static readonly QualificationLevel Level7 = new(7, "Level 7");
    public static readonly QualificationLevel Level8 = new(8, "Level 8");
    public static readonly QualificationLevel Unspecified = new(99, "Unspecified");

    public int Id { get; }
    public string Name { get; set; } = null!;

    public QualificationLevel(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static readonly IReadOnlyCollection<QualificationLevel> All = new List<QualificationLevel>
    {
        EntryLevel, Level1, Level1Or2, Level2, Level3, Level4, Level5, Level6, Level7, Level8
    }.OrderBy(o => o.Name).ToList();

    public static QualificationLevel FromId(int id) => All.FirstOrDefault(x => x.Id == id) ?? Unspecified;

    public static QualificationLevel FromName(string name) => All.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Unspecified;

    public override string ToString() => Name;
}