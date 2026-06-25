using SFA.DAS.AODP.Data.ValueConverters;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.ValueConverters;

public class NullableDateOnlyToDateTimeConverterTests : UnitTest
{

    private readonly NullableDateOnlyToDateTimeConverter _converter = new();

    [Fact]
    public void Convert_ToProvider_WhenValue_Converts_DateOnly_To_DateTime()
    {
        // Arrange
        var input = new DateOnly(2024, 5, 10);

        // Act
        var result = _converter.ConvertToProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBe(new DateTime(2024, 5, 10, 0, 0, 0));
    }

    [Fact]
    public void Convert_ToProvider_WhenNull_Returns_Null()
    {
        // Arrange
        DateOnly? input = null;

        // Act
        var result = _converter.ConvertToProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Convert_FromProvider_WhenValue_Converts_DateTime_To_DateOnly()
    {
        // Arrange
        var input = new DateTime(2024, 5, 10, 15, 30, 0);

        // Act
        var result = _converter.ConvertFromProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBe(new DateOnly(2024, 5, 10));
    }

    [Fact]
    public void Convert_FromProvider_WhenNull_Returns_Null()
    {
        // Arrange
        DateTime? input = null;

        // Act
        var result = _converter.ConvertFromProviderExpression.Compile().Invoke(input);

        // Assert
        result.ShouldBeNull();
    }

}
