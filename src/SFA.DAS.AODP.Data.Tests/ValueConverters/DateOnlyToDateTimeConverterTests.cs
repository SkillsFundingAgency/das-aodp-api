using SFA.DAS.AODP.Data.ValueConverters;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.ValueConverters;

public class DateOnlyToDateTimeConverterTests : UnitTest
{

    private readonly DateOnlyToDateTimeConverter _converter = new();

    [Fact]
    public void Convert_ToProvider_Converts_DateOnly_To_DateTime()
    {
        // Arrange
        var input = new DateOnly(2024, 5, 10);

        // Act
        var result = _converter.ConvertToProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBe(new DateTime(2024, 5, 10, 0, 0, 0));
    }

    [Fact]
    public void Convert_FromProvider_Converts_DateTime_To_DateOnly()
    {
        // Arrange
        var input = new DateTime(2024, 5, 10, 15, 30, 0);

        // Act
        var result = _converter.ConvertFromProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBe(new DateOnly(2024, 5, 10));
    }

}