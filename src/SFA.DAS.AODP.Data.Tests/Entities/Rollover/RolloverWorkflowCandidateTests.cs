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
            currentFundingEndDate,
            proposedFundingEndDate,
            createdAt);
        
        // Assert
        Assert.Equal(workflowRunId, result.RolloverWorkflowRunId);
        Assert.Equal(rolloverCandidateRecordId, result.RolloverCandidateRecordId);
        Assert.Equal(qualificationVersionId, result.QualificationVersionId);
        Assert.Equal(fundingOfferId, result.FundingOfferId);
        Assert.Equal(academicYear, result.AcademicYear);
        Assert.Equal(currentFundingEndDate, result.CurrentFundingEndDate);
        Assert.Equal(proposedFundingEndDate, result.ProposedFundingEndDate);
        Assert.Equal(createdAt, result.CreatedAt);

        // As this is the first entry the created and updated at should be the same.
        Assert.Equal(createdAt, result.UpdatedAt);
        Assert.True(result.IncludedInP1Export);
        Assert.False(result.IncludedInFinalUpload);
        Assert.False(result.PassP1);
        Assert.Null(result.P1FailureReason);
    }

    [Fact]
    public void Create_AcademicYearNull_ShouldThrowException()
    {
        // Arrange
        var workflowRunId = Guid.NewGuid();
        var rolloverCandidateRecordId = Guid.NewGuid();
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var currentFundingEndDate = new DateTime(2026, 02, 28, 12, 00, 00);
        var proposedFundingEndDate = new DateTime(2026, 08, 31, 12, 00, 00);
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RolloverWorkflowCandidate.Create(workflowRunId, rolloverCandidateRecordId, qualificationVersionId, fundingOfferId, null!, currentFundingEndDate, proposedFundingEndDate, createdAt));
    }
}