using SFA.DAS.AODP.Infrastructure.Extensions;

namespace SFA.DAS.AODP.Application.UnitTests.Extensions
{
    public class NullableExtensionsTests
    {

        [Fact]
        public void OrEmpty_ReturnsEmptyString_WhenNull()
        {
            string? value = null;

            var result = value.OrEmpty();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void OrEmpty_ReturnsOriginal_WhenNotNull()
        {
            var result = "hello".OrEmpty();

            Assert.Equal("hello", result);
        }

        [Fact]
        public void OrFalse_ReturnsFalse_WhenNull()
        {
            bool? value = null;

            var result = value.OrFalse();

            Assert.False(result);
        }

        [Fact]
        public void OrFalse_ReturnsValue_WhenNotNull()
        {
            Assert.True(((bool?)true).OrFalse());
            Assert.False(((bool?)false).OrFalse());
        }

        [Fact]
        public void ToIntOrDefault_ReturnsParsedValue_WhenValidInt()
        {
            var result = "123".ToIntOrDefault();

            Assert.Equal(123, result);
        }

        [Fact]
        public void ToIntOrDefault_ReturnsDefault_WhenInvalidInt()
        {
            var result = "abc".ToIntOrDefault(99);

            Assert.Equal(99, result);
        }

        [Fact]
        public void ToIntOrDefault_ReturnsDefault_WhenNull()
        {
            string? value = null;

            var result = value.ToIntOrDefault(42);

            Assert.Equal(42, result);
        }


        [Fact]
        public void ToDateOnlyOrNull_ReturnsNull_WhenNull()
        {
            DateTime? value = null;

            var result = value.ToDateOnlyOrNull();

            Assert.Null(result);
        }

        [Fact]
        public void ToDateOnlyOrNull_ReturnsDateOnly_WhenValuePresent()
        {
            var dt = new DateTime(2024, 5, 10);

            var result = ((DateTime?)dt).ToDateOnlyOrNull();

            Assert.Equal(new DateOnly(2024, 5, 10), result);
        }
    }
}
