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

    [Fact]
    public void AreDatesWithinSameAcademicYear_BothDatesWithinSameAcademicYear()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 1));

        // Act
        var result = sut.AreDatesWithinSameAcademicYear(new DateTime(2025, 08, 1), new DateOnly(2026, 07, 31));

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void AreDatesWithinSameAcademicYear_BothDatesNotWithinSameAcademicYear()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 1));

        // Act
        var result = sut.AreDatesWithinSameAcademicYear(new DateTime(2025, 08, 1), new DateOnly(2026, 08, 01));

        // Assert
        result.ShouldBe(false);
    }

    [Fact]
    public void AreDatesWithinSameAcademicYear_BothDatesAreSameSoAreWithinSameAcademicYear()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 1));

        // Act
        var result = sut.AreDatesWithinSameAcademicYear(new DateTime(2025, 08, 1), new DateOnly(2025, 08, 01));

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void IsDateWithinCurrentAcademicYear_DateTimeIsNull_ReturnFalse()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 1));

        // Act
        var result = sut.IsWithinCurrentAcademicYear(null);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDateWithinCurrentAcademicYear_IsWithinCurrentYear_ReturnTrue()
    {
        // Arrange
        var mockSystemClock = new Mock<ISystemClockProvider>();
        var sut = new AcademicYearProvider(mockSystemClock.Object);

        // Expectations
        mockSystemClock.Setup(o => o.Today).Returns(new DateOnly(2025, 08, 10));

        // Act
        var result = sut.IsWithinCurrentAcademicYear(new DateTime(2026, 02, 01, 12, 00, 00));

        // Assert
        result.ShouldBeTrue();
    }
}