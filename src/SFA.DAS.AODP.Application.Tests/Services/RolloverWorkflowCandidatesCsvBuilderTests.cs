using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Services
{
    public class RolloverWorkflowCandidatesCsvBuilderTests
    {
        private readonly FundingExtensionCandidatesCsvBuilder _sut;

        public RolloverWorkflowCandidatesCsvBuilderTests()
        {
            _sut = new FundingExtensionCandidatesCsvBuilder();
        }

        [Fact]
        public void Build_ShouldIncludeHeaders_AndBasicRowContent()
        {
            // Arrange
            var row = new RolloverCandidateForExport
            {
                QAN = "12345",
                QualificationTitle = "Test Qualification",
                OperationalEndDate = new DateTime(2026, 05, 20, 14, 30, 00),
                FundingApprovalStartDate = new DateOnly(2026, 06, 01),
                OfferedInEngland = true,
                FundedInEngland = false,
                GLH = 120,
                TQT = null
            };

            // Act
            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            // Assert (headers exist)
            Assert.Contains("QAN,QualificationTitle,AwardingOrganisation", csv);

            // Assert key values exist (no row parsing required)
            Assert.Contains("12345", csv);
            Assert.Contains("Test Qualification", csv);
            Assert.Contains("True", csv);
            Assert.Contains("False", csv);
            Assert.Contains("120", csv);
        }

        [Theory]
        [InlineData("Standard Text", "Standard Text")]
        [InlineData("Value, with comma", "\"Value, with comma\"")]
        [InlineData("Value \"with\" quotes", "\"Value \"\"with\"\" quotes\"")]
        [InlineData("Value\nwith\rnewlines", "\"Value\nwith\rnewlines\"")]
        public void Build_ShouldEscapeWeirdCharacters_AccordingToCsvStandards(
            string weirdValue,
            string expectedEscapedOutput)
        {
            // Arrange
            var row = new RolloverCandidateForExport
            {
                QAN = "111",
                QualificationTitle = weirdValue,
                Comments = weirdValue
            };

            // Act
            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            // Assert
            Assert.Contains(expectedEscapedOutput, csv);
        }

        [Fact]
        public void Build_ShouldHandleEmptyCollectionWithoutCrashing()
        {
            // Act
            var csv = Encoding.UTF8.GetString(
                _sut.Build(Enumerable.Empty<RolloverCandidateForExport>()));

            // Assert
            Assert.Contains("QAN,QualificationTitle", csv);
        }
    }
}