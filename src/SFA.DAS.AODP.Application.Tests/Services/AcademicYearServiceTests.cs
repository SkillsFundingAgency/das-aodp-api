using Moq;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Services.FundingExtension
{
    public class AcademicYearServiceTests
    {
        [Fact]
        public void GetCurrentAcademicYear_WhenDateIsJanuary_ReturnsPreviousStartYear()
        {
            var clock = new Mock<ISystemClockService>();
            clock.Setup(x => x.UtcNow).Returns(new DateTime(2026, 1, 15));

            var sut = new AcademicYearService(clock.Object);

            var result = sut.GetCurrentAcademicYear();

            Assert.Equal("2025/26", result);
        }

        [Fact]
        public void GetCurrentAcademicYear_WhenDateIsAugust_ReturnsCurrentStartYear()
        {
            var clock = new Mock<ISystemClockService>();
            clock.Setup(x => x.UtcNow).Returns(new DateTime(2026, 8, 1));

            var sut = new AcademicYearService(clock.Object);

            var result = sut.GetCurrentAcademicYear();

            Assert.Equal("2026/27", result);
        }

        [Fact]
        public void GetCurrentAcademicYear_WhenDateIsJuly_ReturnsPreviousStartYear()
        {
            var clock = new Mock<ISystemClockService>();
            clock.Setup(x => x.UtcNow).Returns(new DateTime(2026, 7, 31));

            var sut = new AcademicYearService(clock.Object);

            var result = sut.GetCurrentAcademicYear();

            Assert.Equal("2025/26", result);
        }

        [Fact]
        public void GetCurrentAcademicYear_WhenDateIsDecember_ReturnsPreviousStartYear()
        {
            var clock = new Mock<ISystemClockService>();
            clock.Setup(x => x.UtcNow).Returns(new DateTime(2026, 12, 10));

            var sut = new AcademicYearService(clock.Object);

            var result = sut.GetCurrentAcademicYear();

            Assert.Equal("2026/27", result);
        }
    }
}

