using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.FundingExtension;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.FundingExtension;

public class FundingExtensionPersistenceRepositoryTests : UnitTest
{
    private static readonly DateTime CreatedAt = new(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void CreateStagingRows_WhenCandidateHasMatchingFundingUpdate_MapsFundingFieldsOntoStagingRow()
    {
        var operationId = Guid.NewGuid();
        var sourceQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            sourceQualificationId,
            fundingOfferId,
            "2025/26",
            CreatedAt);
        candidate.SetExtended(new DateTime(2027, 7, 31));

        var fundingUpdate = new RolloverFundingUpdate(
            Guid.NewGuid(),
            RolloverSourceTypes.Ofqual,
            sourceQualificationId,
            fundingOfferId,
            "2025/26",
            null,
            (_, _, _) => { });
        fundingUpdate.ApplyFundingEndDate(new DateOnly(2027, 7, 31), "Extended for another year", CreatedAt);

        var rows = FundingExtensionPersistenceRepository.CreateStagingRows(
            operationId,
            [candidate],
            [fundingUpdate],
            CreatedAt);

        var row = rows.ShouldHaveSingleItem();
        row.OperationId.ShouldBe(operationId);
        row.RolloverCandidateId.ShouldBe(candidate.Id);
        row.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        row.SourceFundingRecordId.ShouldBe(fundingUpdate.Id);
        row.RolloverStatus.ShouldBe(RolloverStatus.Extended);
        row.NewFundingEndDate.ShouldBe(candidate.NewFundingEndDate);
        row.FundingEndDate.ShouldBe(new DateOnly(2027, 7, 31));
        row.FundingComments.ShouldBe("Extended for another year");
        row.CreatedAt.ShouldBe(CreatedAt);
    }

    [Fact]
    public void CreateStagingRows_WhenCandidateHasNoMatchingFundingUpdate_LeavesFundingFieldsNull()
    {
        var operationId = Guid.NewGuid();
        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2025/26",
            CreatedAt);
        candidate.SetExcluded("No longer offered");

        var rows = FundingExtensionPersistenceRepository.CreateStagingRows(
            operationId,
            [candidate],
            [],
            CreatedAt);

        var row = rows.ShouldHaveSingleItem();
        row.RolloverCandidateId.ShouldBe(candidate.Id);
        row.SourceType.ShouldBeNull();
        row.SourceFundingRecordId.ShouldBeNull();
        row.RolloverStatus.ShouldBe(RolloverStatus.Excluded);
        row.ExclusionReason.ShouldBe("No longer offered");
        row.FundingEndDate.ShouldBeNull();
        row.FundingComments.ShouldBeNull();
    }

    [Fact]
    public void CreateStagingRows_WhenFundingUpdateSourceTypeDiffersFromCandidate_DoesNotCrossMatch()
    {
        // Same SourceQualificationId/FundingOfferId but a different SourceType must not match -
        // the lookup key is the full (SourceType, SourceQualificationId, FundingOfferId) triple.
        var operationId = Guid.NewGuid();
        var sourceQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa,
            sourceQualificationId,
            fundingOfferId,
            "2025/26",
            CreatedAt);

        var decoyOfqualUpdate = new RolloverFundingUpdate(
            Guid.NewGuid(),
            RolloverSourceTypes.Ofqual,
            sourceQualificationId,
            fundingOfferId,
            "2025/26",
            null,
            (_, _, _) => { });

        var rows = FundingExtensionPersistenceRepository.CreateStagingRows(
            operationId,
            [candidate],
            [decoyOfqualUpdate],
            CreatedAt);

        var row = rows.ShouldHaveSingleItem();
        row.SourceType.ShouldBeNull();
        row.SourceFundingRecordId.ShouldBeNull();
    }

    [Fact]
    public void EnsureExpectedRowCount_WhenCountsMatch_DoesNotThrow()
    {
        Should.NotThrow(() =>
            FundingExtensionPersistenceRepository.EnsureExpectedRowCount("rollover candidate", 3, 3));
    }

    [Fact]
    public void EnsureExpectedRowCount_WhenCountsDiffer_ThrowsWithRowTypeAndCounts()
    {
        var exception = Should.Throw<DbUpdateConcurrencyException>(() =>
            FundingExtensionPersistenceRepository.EnsureExpectedRowCount("rollover candidate", 3, 2));

        exception.Message.ShouldContain("rollover candidate");
        exception.Message.ShouldContain("3");
        exception.Message.ShouldContain("2");
    }
}
