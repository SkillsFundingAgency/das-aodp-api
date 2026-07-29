using SFA.DAS.AODP.Models.Rollover;

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
        Assert.Equal(RolloverSourceTypes.Ofqual, result.SourceType);
        Assert.Equal(qualificationVersionId, result.SourceQualificationId);
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
        Assert.Null(result.RolloverDecisionRun);
        Assert.NotEqual(Guid.Empty, result.Id);

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

    [Fact]
    public void DeactivateThenReactivate_ResetsDecisionStateAndRefreshesFunding()
    {
        var createdAt = new DateTime(2026, 2, 28, 12, 0, 0);
        var updatedAt = createdAt.AddDays(1);
        var fundingEndDate = new DateOnly(2027, 7, 31);
        var candidate = RolloverCandidates.CreateInitialRound(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2026/27",
            createdAt);
        candidate.SetExcluded("Previous decision");

        candidate.Deactivate(updatedAt);
        candidate.Reactivate(fundingEndDate, updatedAt);

        Assert.True(candidate.IsActive);
        Assert.Equal(RolloverStatus.NeedsReview, candidate.RolloverStatus);
        Assert.Null(candidate.ExclusionReason);
        Assert.Null(candidate.NewFundingEndDate);
        Assert.Null(candidate.ReviewedAt);
        Assert.Null(candidate.ReviewedByUsername);
        Assert.Null(candidate.RolloverDecisionRunId);
        Assert.Equal(fundingEndDate.ToDateTime(TimeOnly.MinValue), candidate.PreviousFundingEndDate);
        Assert.Equal(updatedAt, candidate.UpdatedAt);
    }
}
