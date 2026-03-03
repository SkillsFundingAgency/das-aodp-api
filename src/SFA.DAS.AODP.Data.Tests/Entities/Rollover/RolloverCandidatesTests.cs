namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverCandidatesTests
{
    [Fact]
    public void CreateInitialRound_Success_EnsureValuesSetCorrectly()
    {
        // Arrange
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var academicYear = "24/25";
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);
        
        // Act
        var result = RolloverCandidates.CreateInitialRound(qualificationVersionId, fundingOfferId, academicYear, createdAt);
        
        // Assert
        Assert.Equal(qualificationVersionId, result.QualificationVersionId);
        Assert.Equal(fundingOfferId, result.FundingOfferId);
        Assert.Equal(academicYear, result.AcademicYear);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(1, result.RolloverRound);
        Assert.True(result.IsActive);
        Assert.Equal(RolloverStatus.NeedsReview, result.RolloverStatus);
        Assert.Null(result.ExclusionReason);
        Assert.Null(result.PreviousFundingEndDate);
        Assert.Null(result.NewFundingEndDate);
        Assert.Null(result.ReviewedAt);
        Assert.Null(result.ReviewedByUsername);
        Assert.Null(result.RolloverDecisionRunId);
        Assert.Null(result.QualificationVersion);
        Assert.Null(result.DecisionRun);
        Assert.Equal(Guid.Empty, result.Id);

        // As this is the first entry the created and updated at should be the same.
        Assert.Equal(createdAt, result.UpdatedAt);
    }

    [Fact]
    public void CreateInitialRound_AcademicYearNull_ShouldThrowException()
    {
        // Arrange
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            RolloverCandidates.CreateInitialRound(qualificationVersionId, fundingOfferId, null!, createdAt));
    }
}