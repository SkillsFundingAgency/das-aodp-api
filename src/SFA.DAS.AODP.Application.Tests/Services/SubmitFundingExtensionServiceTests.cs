using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.UnitTests;
using SFA.DAS.AODP.Application.UnitTests.Helpers;
using SFA.DAS.AODP.Data.Entities.Qualification;
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

    [Fact]
    public async Task Submit_WhenCandidateIsExtended_PersistsCandidateFundingAndHistory()
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
            qualificationId);
        var funding = new QualificationFundings
        {
            QualificationVersionId = qualificationVersionId,
            QualificationVersion = new QualificationVersions
            {
                Id = qualificationVersionId,
                QualificationId = qualificationId
            },
            FundingOfferId = candidate.FundingOfferId
        };

        IReadOnlyCollection<RolloverCandidates>? persistedCandidates = null;
        IReadOnlyCollection<QualificationFundings>? persistedFundings = null;
        IReadOnlyCollection<QualificationDiscussionHistory>? persistedHistories = null;

        _guidProvider.Setup(provider => provider.NewGuid()).Returns(historyId);
        _clockService.Setup(service => service.UtcNow).Returns(timestamp);
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<QualificationFundings>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                CancellationToken))
            .Callback<IReadOnlyCollection<RolloverCandidates>, IReadOnlyCollection<QualificationFundings>,
                IReadOnlyCollection<QualificationDiscussionHistory>, CancellationToken>(
                (candidates, fundings, histories, _) =>
                {
                    persistedCandidates = candidates;
                    persistedFundings = fundings;
                    persistedHistories = histories;
                })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [funding], CancellationToken);

        // Assert
        result.ShouldBeTrue();
        persistedCandidates.ShouldBe([candidate]);
        persistedFundings.ShouldBe([funding]);
        persistedHistories.ShouldNotBeNull();
        persistedHistories.Single().ShouldBeEquivalentTo(new QualificationDiscussionHistory
        {
            Id = historyId,
            QualificationId = qualificationId,
            UserDisplayName = "Rollover System",
            Title = "Rollover Funding Decision",
            Timestamp = timestamp,
            Notes = "FS extended to 06-10-2027",
            ActionTypeId = Guid.Parse("00000000-0000-0000-0000-000000000004")
        });
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Extended);
        funding.EndDate.ShouldBe(new DateOnly(2027, 10, 6));
        funding.Comments.ShouldBe(item.Comments);
    }

    [Fact]
    public async Task Submit_WhenCandidateIsExcluded_PersistsCandidateAndHistoryWithoutFunding()
    {
        // Arrange
        var item = new FundingExtensionItem
        {
            Qan = "111",
            FundingStreamName = "FS",
            RolloverStatus = "Excluded",
            ExclusionReason = "Bad data"
        };
        var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
        IReadOnlyCollection<QualificationFundings>? persistedFundings = null;
        IReadOnlyCollection<QualificationDiscussionHistory>? persistedHistories = null;

        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.Is<IReadOnlyCollection<RolloverCandidates>>(values => values.Count == 1),
                It.IsAny<IReadOnlyCollection<QualificationFundings>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                CancellationToken))
            .Callback<IReadOnlyCollection<RolloverCandidates>, IReadOnlyCollection<QualificationFundings>,
                IReadOnlyCollection<QualificationDiscussionHistory>, CancellationToken>(
                (_, fundings, histories, _) =>
                {
                    persistedFundings = fundings;
                    persistedHistories = histories;
                })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeTrue();
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Excluded);
        candidate.ExclusionReason.ShouldBe("Bad data");
        persistedFundings.ShouldBeEmpty();
        persistedHistories.ShouldNotBeNull();
        persistedHistories.Count.ShouldBe(1);
        persistedHistories.Single().Notes.ShouldBe("FS was not extended due to Bad data");
    }

    [Fact]
    public async Task Submit_WhenStatusesAreMixed_CreatesTwoHistoryEntries()
    {
        // Arrange
        var extendedItem = new FundingExtensionItem
        {
            Qan = "111",
            FundingStreamName = "FS1",
            RolloverStatus = "Extended",
            ProposedFundingApprovalEndDate = new DateTime(2027, 10, 6)
        };
        var excludedItem = new FundingExtensionItem
        {
            Qan = "222",
            FundingStreamName = "FS2",
            RolloverStatus = "Excluded",
            ExclusionReason = "Reason"
        };
        var extendedCandidate = CandidateHelper.BuildCandidate(
            _fixture,
            extendedItem.Qan,
            extendedItem.FundingStreamName);
        var excludedCandidate = CandidateHelper.BuildCandidate(
            _fixture,
            excludedItem.Qan,
            excludedItem.FundingStreamName);
        excludedCandidate.QualificationVersion.QualificationId =
            extendedCandidate.QualificationVersion.QualificationId;
        var funding = new QualificationFundings
        {
            QualificationVersionId = extendedCandidate.QualificationVersionId,
            FundingOfferId = extendedCandidate.FundingOfferId
        };

        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.Is<IReadOnlyCollection<RolloverCandidates>>(values => values.Count == 2),
                It.Is<IReadOnlyCollection<QualificationFundings>>(values => values.Count == 1),
                It.Is<IReadOnlyCollection<QualificationDiscussionHistory>>(values => values.Count == 2),
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit(
            [extendedCandidate, excludedCandidate],
            [extendedItem, excludedItem],
            [funding],
            CancellationToken);

        // Assert
        result.ShouldBeTrue();
        _persistenceRepository.VerifyAll();
    }

    [Fact]
    public async Task Submit_WhenInputDoesNotMatch_PersistsEmptyChangesToCompleteWorkflow()
    {
        // Arrange
        var candidate = CandidateHelper.BuildCandidate(_fixture, "111", "FS");
        var item = new FundingExtensionItem
        {
            Qan = "XXX",
            FundingStreamName = "YYY",
            RolloverStatus = "Extended"
        };

        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.Is<IReadOnlyCollection<RolloverCandidates>>(values => values.Count == 0),
                It.Is<IReadOnlyCollection<QualificationFundings>>(values => values.Count == 0),
                It.Is<IReadOnlyCollection<QualificationDiscussionHistory>>(values => values.Count == 0),
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Submit([candidate], [item], [], CancellationToken);

        // Assert
        result.ShouldBeTrue();
        candidate.RolloverStatus.ShouldBe(RolloverStatus.None);
        _persistenceRepository.VerifyAll();
    }

    [Fact]
    public async Task Submit_WhenPersistenceThrows_ReturnsFalse()
    {
        // Arrange
        _persistenceRepository
            .Setup(repository => repository.PersistAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidates>>(),
                It.IsAny<IReadOnlyCollection<QualificationFundings>>(),
                It.IsAny<IReadOnlyCollection<QualificationDiscussionHistory>>(),
                CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Boom"));

        // Act
        var result = await _service.Submit([], [], [], CancellationToken);

        // Assert
        result.ShouldBeFalse();
        _logger.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("Funding-extension processing failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
