using SFA.DAS.AODP.Data.Extensions;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Application.UnitTests.Extensions;

public class DateTimeExtensionsUnitTests : UnitTest
{
    [Fact]
    public void GetSpecificWorkingDateOfMonth_NoOverlappingNonWorkingDays()
    {
        // Arrange
        var startDate = new DateTime(2024, 10, 1);

        // Act
        var result = startDate.GetSpecificWorkingDateOfMonth(2024, 10, 3);

        // Assert
        result.ShouldBe(new DateTime(2024, 10, 3));
    }

    [Fact]
    public void GetSpecificWorkingDateOfMonth_OverlappingNonWorkingDays()
    {
        // Arrange
        var startDate = new DateTime(2024, 10, 1);

        // Act
        var result = startDate.GetSpecificWorkingDateOfMonth(2024, 10, 10);

        // Assert
        result.ShouldBe(new DateTime(2024, 10, 14));
    }
}