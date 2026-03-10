namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverWorkflowRunFilterTests
{
    [Fact]
    public void Create_EnsureValuesSetCorrectly()
    {
        // Arrange
        var workflowRunId = Guid.NewGuid();
        var filterKey = FilterKey.QualificationType;
        var createdAt = new DateTime(2026, 02, 03, 12, 00, 00);

        // Act
        var result = RolloverWorkflowRunFilter.Create(workflowRunId, filterKey, createdAt);

        // Assert
        Assert.Equal(workflowRunId, result.RolloverWorkflowRunId);
        Assert.Equal(filterKey, result.FilterKey);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Empty(result.Values);
        Assert.Null(result.RolloverWorkflowRun);
        Assert.Equal(Guid.Empty, result.Id);
    }
}