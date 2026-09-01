using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[ExcludeFromCodeCoverage]
public sealed record FundingStream
{
    public Guid Id { get; }
    public string Name { get; }

    private FundingStream(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static readonly FundingStream LegalEntitlementEnglishAndMaths = new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "LegalEntitlementEnglishandMaths");
    public static readonly FundingStream LifelongLearningEntitlement = new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "LifelongLearningEntitlement");
    public static readonly FundingStream LocalFlexibilities = new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "LocalFlexibilities");
    public static readonly FundingStream DigitalEntitlement = new(Guid.Parse("00000000-0000-0000-0000-000000000004"), "DigitalEntitlement");
    public static readonly FundingStream LegalEntitlementL2L3 = new(Guid.Parse("00000000-0000-0000-0000-000000000005"), "LegalEntitlementL2L3");
    public static readonly FundingStream AdvancedLearnerLoans = new(Guid.Parse("00000000-0000-0000-0000-000000000006"), "AdvancedLearnerLoans");
    public static readonly FundingStream Age1619 = new(Guid.Parse("00000000-0000-0000-0000-000000000007"), "Age1619");
    public static readonly FundingStream FreeCoursesForJobs = new(Guid.Parse("00000000-0000-0000-0000-000000000008"), "FreeCoursesForJobs");
    public static readonly FundingStream Age1416 = new(Guid.Parse("00000000-0000-0000-0000-000000000009"), "Age1416");

    public static IReadOnlyCollection<FundingStream> All { get; } =
    [
        LegalEntitlementEnglishAndMaths,
        LifelongLearningEntitlement,
        LocalFlexibilities,
        DigitalEntitlement,
        LegalEntitlementL2L3,
        AdvancedLearnerLoans,
        Age1619,
        FreeCoursesForJobs,
        Age1416
    ];

    // Lookup by Guid
    public static FundingStream FromId(Guid id) =>
        All.FirstOrDefault(x => x.Id == id)
        ?? throw new ArgumentException($"Unknown FundingType Id: {id}");

    // Lookup by Name
    public static FundingStream FromName(string name) =>
        All.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"Unknown FundingType Name: {name}");

    public override string ToString() => Name;
}