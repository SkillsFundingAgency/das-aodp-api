namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverWorkflowCandidateTests
{
    [Fact]
    public void Create_Success_EnsureValuesSetCorrectly()
    {
        var workflowRunId = Guid.NewGuid();
        var rolloverCandidateRecordId = Guid.NewGuid();
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var academicYear = "2025/26";
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
        Assert.Throws<ArgumentNullException>(() =>
            RolloverWorkflowCandidate.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                null!,
                1,
                DateTime.UtcNow,
                null,
                DateTime.UtcNow));
    }

    [Fact]
    public void ProcessP1Checks_WhenAllValid_SetsPassP1True()
    {
        var candidate = CreateCandidate();
        var checks = CreateValidChecks();

        candidate.ProcessP1Checks(checks);

        Assert.True(candidate.PassP1);
        Assert.Null(candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_WhenOfferedInEnglandFalse_SetsFailure()
    {
        var candidate = CreateCandidate();
        var checks = CreateValidChecks();
        checks.OfferedInEngland = false;

        candidate.ProcessP1Checks(checks);

        Assert.False(candidate.PassP1);
        Assert.Contains("Not Offered in England", candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_WhenMultipleFailures_AggregatesReasons()
    {
        var candidate = CreateCandidate();

        var checks = new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = null,
            OfferedInEngland = false,
            IntentionToSeekFundingInEngland = false,
            IsOnDefundingList = true,
            FundingEndDateThreshold = new DateTime(2025, 01, 01),
            OperationalEndDateThreshold = new DateTime(2025, 01, 01),
            OperationalEndDate = new DateTime(2024, 01, 01),
            LatestFundingApprovalEndDate = new DateTime(2024, 01, 01)
        };

        candidate.ProcessP1Checks(checks);

        Assert.False(candidate.PassP1);
        Assert.Contains("Funding Stream out of scope", candidate.P1FailureReason);
        Assert.Contains("Not Offered in England", candidate.P1FailureReason);
        Assert.Contains("Not Funded in England", candidate.P1FailureReason);
        Assert.Contains("Qualification is on Defunding", candidate.P1FailureReason);
    }

    [Fact]
    public void ProcessP1Checks_UpdatesUpdatedAt()
    {
        var candidate = CreateCandidate();
        var before = DateTime.UtcNow;
        var checks = CreateValidChecks();

        candidate.ProcessP1Checks(checks);

        Assert.InRange(candidate.UpdatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public void ProcessP1Checks_WhenCandidateFails_ResetsProposedFundingEndDateToCurrentFundingEndDate()
    {
        var currentFundingEndDate = new DateTime(2026, 01, 31);

        var candidate = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2025/26",
            1,
            currentFundingEndDate,
            new DateTime(2027, 01, 01),
            DateTime.UtcNow);

        var checks = CreateValidChecks();
        checks.FundingStream = null;

        candidate.ProcessP1Checks(checks);

        Assert.False(candidate.PassP1);
        Assert.Equal(currentFundingEndDate, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenPldnsBeforeMaxEndDate_UsesPldns()
    {
        var candidate = CreateCandidate();

        var latestEndDate = new DateTime(2025, 08, 01);
        var operationalEndDate = new DateTime(2025, 06, 01);
        var maxEndDate = new DateTime(2025, 08, 01);
        var pldns = new DateTime(2025, 05, 01);

        var checks = CreateValidChecks(
            latestEndDate: latestEndDate,
            operationalEndDate: operationalEndDate,
            pldns: pldns,
            maxFundingEndDate: maxEndDate);

        candidate.ProcessP1Checks(checks);

        Assert.Equal(pldns, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenOpEndDateBeforeMaxEndDate_AndSameAcademicYear_UsesMaxEndDate()
    {
        var candidate = CreateCandidate();

        var latestEndDate = new DateTime(2025, 06, 30);
        var operationalEndDate = new DateTime(2025, 08, 01); // inside academic year
        var maxEndDate = new DateTime(2025, 09, 01);
        var pldns = (DateTime?)null;

        var checks = CreateValidChecks(
            latestEndDate: latestEndDate,
            operationalEndDate: operationalEndDate,
            pldns: pldns,
            maxFundingEndDate: maxEndDate);

        candidate.ProcessP1Checks(checks);

        Assert.Equal(maxEndDate, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenOpEndDateBeforeMaxEndDate_AndDifferentAcademicYear_UsesOpEndDate()
    {
        var candidate = CreateCandidate();

        var latestEndDate = new DateTime(2025, 08, 01);
        var operationalEndDate = new DateTime(2024, 12, 01); // previous academic year
        var maxEndDate = new DateTime(2025, 08, 01);
        var pldns = (DateTime?)null;

        var checks = CreateValidChecks(
            latestEndDate: latestEndDate,
            operationalEndDate: operationalEndDate,
            pldns: pldns,
            maxFundingEndDate: maxEndDate);

        candidate.ProcessP1Checks(checks);

        Assert.Equal(operationalEndDate, candidate.ProposedFundingEndDate);
    }

    [Fact]
    public void ProcessP1Checks_WhenNoValidPldns_AndOpEndDateNotBeforeMaxEndDate_UsesMaxEndDate()
    {
        var candidate = CreateCandidate();

        var latestEndDate = new DateTime(2026, 08, 31);
        var operationalEndDate = new DateTime(2026, 10, 01); // after maxEndDate
        var maxEndDate = new DateTime(2026, 08, 31);
        var pldns = new DateTime(2027, 01, 01); // invalid because > maxEndDate

        var checks = CreateValidChecks(
            latestEndDate: latestEndDate,
            operationalEndDate: operationalEndDate,
            pldns: pldns,
            maxFundingEndDate: maxEndDate);

        candidate.ProcessP1Checks(checks);

        Assert.Equal(maxEndDate, candidate.ProposedFundingEndDate);
    }


    private static RolloverWorkflowCandidate CreateCandidate(DateTime? proposedFundingEndDate = null)
    {
        return RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2025/26",
            1,
            new DateTime(2026, 01, 31),
            proposedFundingEndDate,
            DateTime.UtcNow);
    }

    private static RolloverWorkflowCandidatesP1Checks CreateValidChecks(
        DateTime? latestEndDate = null,
        DateTime? operationalEndDate = null,
        DateTime? pldns = null,
        DateTime? maxFundingEndDate = null)
    {
        return new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = "Age1416",
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true,
            IsOnDefundingList = false,

            FundingEndDateThreshold = DateTime.MinValue,
            OperationalEndDateThreshold = DateTime.MinValue,

            LatestFundingApprovalEndDate = latestEndDate,
            OperationalEndDate = operationalEndDate,
            MaximumApprovalEndDate = maxFundingEndDate,
            Age1416 = pldns,

            AcademicYear = "2025/26"
        };
    }

}
