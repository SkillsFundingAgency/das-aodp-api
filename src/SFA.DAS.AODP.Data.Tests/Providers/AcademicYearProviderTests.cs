using Moq;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Providers;

public class AcademicYearProviderTests : UnitTest
{
    [Fact]
    public void GetCurrentAcademicYearEndDate_CurrentDateIsBeforeThirtyFirstJuly_ReturnCurrentYearsAcademicEndDate()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 07, 1));

        // Act
        var result = sut.GetCurrentAcademicYearEndDate();

        // Assert
        result.ShouldBe(new DateOnly(2025, 07, 31));
    }

    [Fact]
    public void GetCurrentAcademicYearEndDate_CurrentDateIsAfterThirtyFirstJuly_ReturnNextYearsAcademicEndDate()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 1));

        // Act
        var result = sut.GetCurrentAcademicYearEndDate();

        // Assert
        result.ShouldBe(new DateOnly(2026, 07, 31));
    }
}