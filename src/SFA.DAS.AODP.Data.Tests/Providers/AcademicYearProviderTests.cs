using Moq;
using SFA.DAS.AODP.Data.Providers;

namespace SFA.DAS.AODP.Data.UnitTests.Providers;

public class AcademicYearProviderTests
{
    [Fact]
    public void GetCurrentAcademicYearEndDate_CurrentDateIsBeforeThirtyFirstJuly_ReturnCurrentYearsAcademicEndDate()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.UtcNow).Returns(new DateTime(2025, 07, 1));

        // Act
        var result = sut.GetCurrentAcademicYearEndDate();

        // Assert
        Assert.Equal(new DateOnly(2025, 07, 31), result);
    }

    [Fact]
    public void GetCurrentAcademicYearEndDate_CurrentDateIsAfterThirtyFirstJuly_ReturnNextYearsAcademicEndDate()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.UtcNow).Returns(new DateTime(2025, 08, 1));

        // Act
        var result = sut.GetCurrentAcademicYearEndDate();

        // Assert
        Assert.Equal(new DateOnly(2026, 07, 31), result);
    }
}