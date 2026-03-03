namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverWorkflowRunTests
{
    [Fact]
    public void Create_EnsureValuesSetCorrectly()
    {
        // Arrange
        var academicYear = "24/25";
        var selectionMethod = SelectionMethod.QueryBuilder;
        var fundingEndDateEligibilityThreshold = new DateTime(2026, 12, 31, 12, 00, 00);
        var operationalEndDateEligibilityThreshold = new DateTime(2026, 12, 30, 12, 00, 00);
        var maximumApprovalFundingEndDate = new DateTime(2026, 12, 29, 12, 00, 00);
        var createdByUsername = "test.user";
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);
        
        // Act
        var result = RolloverWorkflowRun.Create(academicYear, selectionMethod, fundingEndDateEligibilityThreshold,
            operationalEndDateEligibilityThreshold, maximumApprovalFundingEndDate, createdByUsername, createdAt);

        // Assert
        Assert.Equal(academicYear, result.AcademicYear);
        Assert.Equal(selectionMethod, result.SelectionMethod);
        Assert.Equal(fundingEndDateEligibilityThreshold, result.FundingEndDateEligibilityThreshold);
        Assert.Equal(operationalEndDateEligibilityThreshold, result.OperationalEndDateEligibilityThreshold);
        Assert.Equal(maximumApprovalFundingEndDate, result.MaximumApprovalFundingEndDate);
        Assert.Equal(createdByUsername, result.CreatedByUsername);
        Assert.Equal(createdAt, result.CreatedAt);

        // These will be empty as part of the unit tests.
        Assert.Empty(result.FundingOffers);
        Assert.Empty(result.Candidates);
        Assert.Empty(result.Filters);
    }

    [Fact]
    public void Create_AcademicYearNull_ShouldThrowException()
    {
        // Arrange
        var selectionMethod = SelectionMethod.QueryBuilder;
        var fundingEndDateEligibilityThreshold = new DateTime(2026, 12, 31, 12, 00, 00);
        var operationalEndDateEligibilityThreshold = new DateTime(2026, 12, 30, 12, 00, 00);
        var maximumApprovalFundingEndDate = new DateTime(2026, 12, 29, 12, 00, 00);
        var createdByUsername = "test.user";
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RolloverWorkflowRun.Create(null!, selectionMethod, fundingEndDateEligibilityThreshold,
            operationalEndDateEligibilityThreshold, maximumApprovalFundingEndDate, createdByUsername, createdAt));
    }

    [Fact]
    public void Create_CreatedByUsernameNull_ShouldThrowException()
    {
        // Arrange
        var academicYear = "24/25";
        var selectionMethod = SelectionMethod.QueryBuilder;
        var fundingEndDateEligibilityThreshold = new DateTime(2026, 12, 31, 12, 00, 00);
        var operationalEndDateEligibilityThreshold = new DateTime(2026, 12, 30, 12, 00, 00);
        var maximumApprovalFundingEndDate = new DateTime(2026, 12, 29, 12, 00, 00);
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RolloverWorkflowRun.Create(academicYear, selectionMethod, fundingEndDateEligibilityThreshold,
            operationalEndDateEligibilityThreshold, maximumApprovalFundingEndDate, null!, createdAt));
    }
}