using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Testing.Stubs;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Providers;

public class IlrSubmissionDeadlinesProviderTests : UnitTest
{
    [Fact]
    public void GetFinalSubmissionDeadline_ReturnsR14Period()
    {
        // Arrange
        var sut = new IlrSubmissionDeadlinesProvider(new FakeSystemClockProvider(new DateOnly(2025, 6, 12)));

        // Act
        var result = sut.GetFinalSubmissionDeadline();

        // Assert
        result.Period.ShouldBe("R14");
    }

    [Theory]
    [InlineData(2025, 10, 16)]
    [InlineData(2026, 10, 22)]
    [InlineData(2027, 10, 21)]
    public void GetFinalSubmissionDeadline_ReturnsDeadlineForCurrentClockYear(int clockYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var today = new DateOnly(clockYear, 6, 12);
        var sut = new IlrSubmissionDeadlinesProvider(new FakeSystemClockProvider(today));

        // Act
        var result = sut.GetFinalSubmissionDeadline();

        // Assert
        result.Date.ShouldBe(new DateOnly(clockYear, expectedMonth, expectedDay));
    }

    [Fact]
    public void GetFinalSubmissionDeadline_UsesClockYearRatherThanCurrentSystemDate()
    {
        // Arrange
        var sut = new IlrSubmissionDeadlinesProvider(new FakeSystemClockProvider(new DateOnly(2030, 1, 1)));

        // Act
        var result = sut.GetFinalSubmissionDeadline();

        // Assert
        result.Date.Year.ShouldBe(2030);
    }

    [Fact]
    public void GetFinalSubmissionDeadline_ReturnsExpectedDeadlineWhenThirdWorkingDateFallsOnMonday()
    {
        // Arrange
        var sut = new IlrSubmissionDeadlinesProvider(new FakeSystemClockProvider(new DateOnly(2026, 1, 1)));

        // Act
        var result = sut.GetFinalSubmissionDeadline();

        // Assert
        result.ShouldBe(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
    }

}