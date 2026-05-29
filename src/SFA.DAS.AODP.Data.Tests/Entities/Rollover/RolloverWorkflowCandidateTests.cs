namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverWorkflowCandidateTests
{
    [Fact]
    public void Create_Success_EnsureValuesSetCorrectly()
    {
        // Arrange
        var workflowRunId = Guid.NewGuid();
        var rolloverCandidateRecordId = Guid.NewGuid();
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var academicYear = "24/25";
        var currentFundingEndDate = new DateTime(2026, 02, 28);
        var proposedFundingEndDate = new DateTime(2026, 08, 31);
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        // Act
        var result = RolloverWorkflowCandidate.Create(
            workflowRunId,
            rolloverCandidateRecordId,
            qualificationVersionId,
            fundingOfferId,
            academicYear,
            1,
            currentFundingEndDate,
            proposedFundingEndDate,
            createdAt);

        // Assert
        Assert.Equal(workflowRunId, result.RolloverWorkflowRunId);
        Assert.Equal(rolloverCandidateRecordId, result.RolloverCandidatesId);
        Assert.Equal(qualificationVersionId, result.QualificationVersionId);
        Assert.Equal(fundingOfferId, result.FundingOfferId);
        Assert.Equal(academicYear, result.AcademicYear);
        Assert.Equal(currentFundingEndDate, result.CurrentFundingEndDate);
        Assert.Equal(proposedFundingEndDate, result.ProposedFundingEndDate);
        Assert.Equal(createdAt, result.CreatedAt);

        Assert.Equal(createdAt, result.UpdatedAt);
        Assert.True(result.IncludedInP1Export);
        Assert.False(result.IncludedInFinalUpload);
        Assert.False(result.PassP1);
        Assert.Null(result.P1FailureReason);
        Assert.Null(result.RolloverWorkflowRun);
        Assert.Null(result.RolloverCandidates);
        Assert.Equal(Guid.Empty, result.Id);
    }

    [Fact]
    public void Create_AcademicYearNull_ShouldThrowException()
    {
        var workflowRunId = Guid.NewGuid();
        var rolloverCandidateRecordId = Guid.NewGuid();
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var currentFundingEndDate = new DateTime(2026, 02, 28, 12, 00, 00);
        var proposedFundingEndDate = new DateTime(2026, 08, 31, 12, 00, 00);
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);
        var rolloverRound = 1;

        Assert.Throws<ArgumentNullException>(() =>
            RolloverWorkflowCandidate.Create(
                workflowRunId,
                rolloverCandidateRecordId,
                qualificationVersionId,
                fundingOfferId,
                null!,
                rolloverRound,
                currentFundingEndDate,
                proposedFundingEndDate,
                createdAt));
    }

    [Fact]
    public void ProcessP1Checks_WhenAllValid_SetsPassP1True()
    {
        // Arrange
        var candidate = CreateCandidate();

        var checks = CreateValidChecks();

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.True(candidate.PassP1);
        Assert.Null(candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_WhenOfferedInEnglandFalse_SetsFailure()
    {
        // Arrange
        var candidate = CreateCandidate();

        var checks = CreateValidChecks();
        checks.OfferedInEngland = false;

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.False(candidate.PassP1);
        Assert.NotNull(candidate.P1FailureReason);
        Assert.Contains("Not Offered in England", candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_WhenMultipleFailures_AggregatesReasons()
    {
        // Arrange
        var candidate = CreateCandidate();

        var checks = new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = null,
            OfferedInEngland = false,
            IntentionToSeekFundingInEngland = false,
            Glh = 50,
            Tqt = 10,
            IsOnDefundingList = true,
            FundingEndDateThreshold = new DateTime(2025, 01, 01),
            OperationalEndDateThreshold = new DateTime(2025, 01, 01),
            OperationalEndDate = new DateTime(2024, 01, 01),
            LatestFundingApprovalEndDate = new DateTime(2024, 01, 01)
        };

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.False(candidate.PassP1);
        Assert.NotNull(candidate.P1FailureReason);

        Assert.Contains("Funding Stream out of scope", candidate.P1FailureReason);
        Assert.Contains("Not Offered in England", candidate.P1FailureReason);
        Assert.Contains("Not Funded in England", candidate.P1FailureReason);
        Assert.Contains("GLH > TQT", candidate.P1FailureReason);
        Assert.Contains("Qualification is on Defunding", candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_UpdatesUpdatedAt()
    {
        // Arrange
        var candidate = CreateCandidate();

        var before = DateTime.UtcNow;

        var checks = CreateValidChecks();

        // Act
        candidate.ProcessP1Checks(checks);

        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(candidate.UpdatedAt, before, after);
    }

    [Fact]
    public void ProcessP1Checks_WhenCandidateFails_ResetsProposedFundingEndDateToCurrentFundingEndDate()
    {
        // Arrange
        var currentFundingEndDate = new DateTime(2026, 01, 31);

        var candidate = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2025",
            1,
            currentFundingEndDate,
            new DateTime(2027, 01, 01),
            DateTime.UtcNow);

        var checks = CreateValidChecks();
        checks.FundingStream = null; // force failure

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.False(candidate.PassP1);
        Assert.Equal(currentFundingEndDate, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenPldnsIsEarliest_SetsPldnsAsProposedFundingEndDate()
    {
        // Arrange
        var candidate = CreateCandidate();

        var pldns = new DateTime(2025, 06, 01);
        var operational = new DateTime(2025, 07, 01);
        var latest = new DateTime(2025, 08, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: latest,
            operationalEndDate: operational,
            pldns: pldns);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(pldns, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenOperationalEndDateIsEarliest_SetsOperationalAsProposedFundingEndDate()
    {
        // Arrange
        var candidate = CreateCandidate();

        var operational = new DateTime(2025, 06, 01);
        var latest = new DateTime(2025, 07, 01);
        var pldns = new DateTime(2025, 08, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: latest,
            operationalEndDate: operational,
            pldns: pldns);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(operational, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenLatestFundingApprovalEndDateIsEarliest_SetsLatestAsProposedFundingEndDate()
    {
        // Arrange
        var candidate = CreateCandidate();

        var latest = new DateTime(2025, 06, 01);
        var operational = new DateTime(2025, 07, 01);
        var pldns = new DateTime(2025, 08, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: latest,
            operationalEndDate: operational,
            pldns: pldns);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(latest, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenNoPldns_UsesEarliestRemainingDate()
    {
        // Arrange
        var candidate = CreateCandidate();

        var operational = new DateTime(2025, 06, 01);
        var latest = new DateTime(2025, 07, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: latest,
            operationalEndDate: operational,
            pldns: null);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(operational, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenPldnsOutsideAcademicYear_IgnoresPldns()
    {
        // Arrange
        var candidate = CreateCandidate();

        var latest = new DateTime(2025, 06, 01);
        var operational = new DateTime(2025, 07, 01);

        // outside 2025/26 academic year
        var pldns = new DateTime(2026, 09, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: latest,
            operationalEndDate: operational,
            pldns: pldns);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(latest, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenDatesAreEqual_StillChoosesMinimumDate()
    {
        // Arrange
        var candidate = CreateCandidate();

        var sharedDate = new DateTime(2025, 06, 01);

        var checks = CreateValidChecks(
            latestFundingApprovalEndDate: sharedDate,
            operationalEndDate: sharedDate,
            pldns: sharedDate);

        // Act
        candidate.ProcessP1Checks(checks);

        // Assert
        Assert.Equal(sharedDate, candidate.ProposedFundingEndDate);
    }

    private static RolloverWorkflowCandidate CreateCandidate(
        DateTime? proposedFundingEndDate = null)
    {
        return RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2025",
            1,
            new DateTime(2026, 01, 31),
            proposedFundingEndDate,
            DateTime.UtcNow);
    }

    private static RolloverWorkflowCandidatesP1Checks CreateValidChecks(
        DateTime? latestFundingApprovalEndDate = null,
        DateTime? operationalEndDate = null,
        DateTime? pldns = null)
    {
        return new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = "Age1416",
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true,
            Glh = 10,
            Tqt = 20,
            IsOnDefundingList = false,

            FundingEndDateThreshold = new DateTime(2025, 01, 01),
            OperationalEndDateThreshold = new DateTime(2024, 12, 01),

            LatestFundingApprovalEndDate =
                latestFundingApprovalEndDate ?? new DateTime(2026, 01, 01),

            OperationalEndDate =
                operationalEndDate ?? new DateTime(2026, 01, 01),

            AcademicYear = "2025/26",

            Age1416 = pldns
        };
    }
}