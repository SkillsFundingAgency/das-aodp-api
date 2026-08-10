using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.UnitTests;
using SFA.DAS.AODP.Application.UnitTests.Helpers;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.FundingExtension;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Application.Tests.Services.Rollover;

public class SubmitFundingExtensionServiceTests : UnitTest
{
    private readonly Mock<IFundingExtensionPersistenceRepository> _persistenceRepository = new();
    private readonly Mock<ISystemClockService> _clockService = new();
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly Mock<ILogger<SubmitFundingExtensionService>> _logger = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly SubmitFundingExtensionService _service;

    public SubmitFundingExtensionServiceTests()
    {
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _service = new SubmitFundingExtensionService(
            _persistenceRepository.Object,
            _clockService.Object,
            _guidProvider.Object,
            _logger.Object);
    }

    private static RolloverFundingUpdate BuildFundingUpdate(
        string sourceType,
        Guid sourceQualificationId,
        Guid fundingOfferId,
        string academicYear,
        out Action appliedRecorder)
    {
        var applied = false;
        var update = new RolloverFundingUpdate(
            Guid.NewGuid(),
            sourceType,
            sourceQualificationId,
            fundingOfferId,
            academicYear,
            null,
            (_, _, _) => applied = true);

        appliedRecorder = () => applied.ShouldBeTrue("expected the underlying funding record's apply delegate to have been invoked");
        return update;
    }

    [Fact]
    public async Task Submit_WhenOfqualCandidateIsExtended_PersistsCandidateFundingAndHistory()
    {
        // Arrange
        var item = new FundingExtensionItem
        {
            Qan = "111",
            FundingStreamName = "FS",
            RolloverStatus = "Extended",
            ProposedFundingApprovalEndDate = new DateTime(2027, 10, 6),
            Comments = "Test comment"
        };

        var historyId = Guid.NewGuid();
        var qualificationId = Guid.NewGuid();
        var qualificationVersionId = Guid.NewGuid();
        var timestamp = new DateTime(2026, 10, 1, 12, 0, 0);
        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            qualificationVersionId,
            qualificationId,
            RolloverSourceTypes.Ofqual);

        var fundingUpdate = BuildFundingUpdate(
            RolloverSourceTypes.Ofqual,
            qualificationVersionId,
            candidate.FundingOfferId,
            candidate.AcademicYear,
            out var assertApplied);

        IReadOnlyCollection<RolloverCandidates>? persistedCandidates = null;
        IReadOnlyCollection<RolloverFundingUpdate>? persistedFundingUpdates = null;
        IReadOnlyCollection<QualificationDiscussionHistory>? persistedHistories = null;

        _guidProvider.Setup(provider => provider.NewGuid()).Returns(historyId);
        _clockService.Setup(service => service.UtcNow).Returns(timestamp);
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
                CancellationToken))
            .Callback<IReadOnlyCollection<RolloverCandidates>, IReadOnlyCollection<RolloverFundingUpdate>,
                IReadOnlyCollection<QualificationDiscussionHistory>, IReadOnlyCollection<QaaQualificationDiscussionHistory>, CancellationToken>(
                (candidates, fundingUpdates, histories, _, _) =>
                {
                    persistedCandidates = candidates;
                    persistedFundingUpdates = fundingUpdates;
                    persistedHistories = histories;
                })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [fundingUpdate], CancellationToken);

        // Assert
        result.ShouldBeTrue();

        candidate.RolloverStatus.ShouldBe(RolloverStatus.Extended);
        assertApplied();

        persistedCandidates.ShouldNotBeNull();
        persistedCandidates!.ShouldContain(candidate);

        persistedFundingUpdates.ShouldNotBeNull();
        persistedFundingUpdates!.ShouldContain(fundingUpdate);

        persistedHistories.ShouldNotBeNull();
        persistedHistories!.Count.ShouldBe(1);
        persistedHistories.Single().Id.ShouldBe(historyId);
        persistedHistories.Single().QualificationId.ShouldBe(qualificationId);
        persistedHistories.Single().Notes.ShouldContain(item.FundingStreamName);
    }

    [Fact]
    public async Task Submit_WhenQaaCandidateIsExtended_MatchesFundingUpdateBySourceTypeAndPersists()
    {
        // Arrange - QAA candidates are matched by SourceType, not just SourceQualificationId,
        // so this confirms the lookup does not accidentally cross-match an Ofqual funding update
        // with a QAA candidate that happens to share a Guid pattern.
        var item = new FundingExtensionItem
        {
            Qan = "AC1234",
            FundingStreamName = "FS",
            RolloverStatus = "Extended",
            ProposedFundingApprovalEndDate = new DateTime(2027, 10, 6),
            Comments = "QAA comment"
        };

        var historyId = Guid.NewGuid();
        var qaaQualificationId = Guid.NewGuid();
        var timestamp = new DateTime(2026, 10, 1, 12, 0, 0);
        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            qaaQualificationId,
            qualificationId: qaaQualificationId,
            sourceType: RolloverSourceTypes.Qaa);

        var qaaFundingUpdate = BuildFundingUpdate(
            RolloverSourceTypes.Qaa,
            qaaQualificationId,
            candidate.FundingOfferId,
            candidate.AcademicYear,
            out var assertApplied);

        // A same-keyed Ofqual update should never be matched to this QAA candidate
        var decoyOfqualUpdate = BuildFundingUpdate(
            RolloverSourceTypes.Ofqual,
            qaaQualificationId,
            candidate.FundingOfferId,
            candidate.AcademicYear,
            out var assertDecoyNotApplied);

        _guidProvider.Setup(provider => provider.NewGuid()).Returns(historyId);
        _clockService.Setup(service => service.UtcNow).Returns(timestamp);
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit(
            [candidate],
            [item],
            [qaaFundingUpdate, decoyOfqualUpdate],
            CancellationToken);

        // Assert
        result.ShouldBeTrue();
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Extended);
        assertApplied();
        Should.Throw<Exception>(() => assertDecoyNotApplied());
    }

    [Fact]
    public async Task Submit_WhenCandidateIsExcluded_SetsExclusionReasonAndPersistsHistoryWithoutFundingUpdate()
    {
        // Arrange
        var item = new FundingExtensionItem
        {
            Qan = "222",
            FundingStreamName = "FS",
            RolloverStatus = "Excluded",
            ExclusionReason = "No longer offered"
        };

        var qualificationId = Guid.NewGuid();
        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            Guid.NewGuid(),
            qualificationId,
            RolloverSourceTypes.Ofqual);

        _clockService.Setup(service => service.UtcNow).Returns(DateTime.UtcNow);
        _guidProvider.Setup(provider => provider.NewGuid()).Returns(Guid.NewGuid());

        IReadOnlyCollection<QualificationDiscussionHistory>? persistedHistories = null;
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
                CancellationToken))
            .Callback<IReadOnlyCollection<RolloverCandidates>, IReadOnlyCollection<RolloverFundingUpdate>,
                IReadOnlyCollection<QualificationDiscussionHistory>, IReadOnlyCollection<QaaQualificationDiscussionHistory>, CancellationToken>(
                (_, _, histories, _, _) => persistedHistories = histories)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeTrue();
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Excluded);
        candidate.ExclusionReason.ShouldBe(item.ExclusionReason);

        persistedHistories.ShouldNotBeNull();
        persistedHistories!.Single().Notes.ShouldContain(item.ExclusionReason);
    }

    [Fact]
    public async Task Submit_WhenCandidateStatusIsUnrecognised_ReturnsFalseAndDoesNotPersist()
    {
        // Arrange
        var item = new FundingExtensionItem
        {
            Qan = "333",
            FundingStreamName = "FS",
            RolloverStatus = "NotARealStatus"
        };

        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            Guid.NewGuid(),
            Guid.NewGuid(),
            RolloverSourceTypes.Ofqual);

        _clockService.Setup(service => service.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeFalse();
        _persistenceRepository.Verify(repository => repository.PersistAsync(
            It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
            It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
            It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
            It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Submit_WhenPersistenceThrows_ReturnsFalse()
    {
        // Arrange
        var item = new FundingExtensionItem
        {
            Qan = "444",
            FundingStreamName = "FS",
            RolloverStatus = "Excluded",
            ExclusionReason = "Test"
        };

        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            Guid.NewGuid(),
            Guid.NewGuid(),
            RolloverSourceTypes.Ofqual);

        _clockService.Setup(service => service.UtcNow).Returns(DateTime.UtcNow);
        _guidProvider.Setup(provider => provider.NewGuid()).Returns(Guid.NewGuid());
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
                CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Persistence failed."));

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task Submit_WhenCandidateHasNoDiscussionQualificationId_IsExcludedFromHistoryGrouping()
    {
        // Arrange - a candidate whose DiscussionQualificationId was never set (SetSourceContext
        // not called with a value) should not blow up grouping, just be skipped for history.
        var item = new FundingExtensionItem
        {
            Qan = "555",
            FundingStreamName = "FS",
            RolloverStatus = "Excluded",
            ExclusionReason = "Test"
        };

        var candidate = CandidateHelper.BuildCandidate(
            _fixture,
            item.Qan,
            item.FundingStreamName,
            Guid.NewGuid(),
            qualificationId: null,
            sourceType: RolloverSourceTypes.Qaa);

        _clockService.Setup(service => service.UtcNow).Returns(DateTime.UtcNow);
        _guidProvider.Setup(provider => provider.NewGuid()).Returns(Guid.NewGuid());

        IReadOnlyCollection<QualificationDiscussionHistory>? persistedHistories = null;
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<RolloverFundingUpdate>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                It.IsAny<IReadOnlyCollection<QaaQualificationDiscussionHistory>>(),
                CancellationToken))
            .Callback<IReadOnlyCollection<RolloverCandidates>, IReadOnlyCollection<RolloverFundingUpdate>,
                IReadOnlyCollection<QualificationDiscussionHistory>, IReadOnlyCollection<QaaQualificationDiscussionHistory>, CancellationToken>(
                (_, _, histories, _, _) => persistedHistories = histories)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeTrue();
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Excluded);
    }
}
