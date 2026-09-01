using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Funding;

public interface IFundingDomainEventSource
{
    IReadOnlyCollection<FundingDomainEvent> FundingDomainEvents { get; }

    void ClearFundingDomainEvents();
}

[ExcludeFromCodeCoverage]
public abstract record FundingDomainEvent;

[ExcludeFromCodeCoverage]
public sealed record FundingChangedDomainEvent(
    string SourceType,
    Guid SourceQualificationId,
    Guid FundingOfferId,
    Guid? PreviousSourceQualificationId = null) : FundingDomainEvent;

[ExcludeFromCodeCoverage]
public sealed record QualificationFundingEligibilityChangedDomainEvent(
    string SourceType,
    Guid SourceQualificationId) : FundingDomainEvent;
