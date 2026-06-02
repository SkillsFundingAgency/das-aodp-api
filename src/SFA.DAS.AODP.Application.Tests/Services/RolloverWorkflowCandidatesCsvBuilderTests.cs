
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Entities.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Services
{

    public class RolloverWorkflowCandidatesCsvBuilderTests
    {
        private readonly RolloverWorkflowCandidatesCsvBuilder _sut;

        public RolloverWorkflowCandidatesCsvBuilderTests()
        {
            _sut = new RolloverWorkflowCandidatesCsvBuilder();
        }

        [Fact]
        public void Build_ShouldCorrectlyFormatSpecialDataTypes_AndIncludeHeaders()
        {
            // Arrange
            var row = new RolloverWorkflowCandidatesExportRow
            {
                QAN = "12345",
                QualificationTitle = "Test Qualification",
                OperationalEndDate = new DateTime(2026, 05, 20, 14, 30, 00), // DateTime
                FundingApprovalStartDate = new DateOnly(2026, 06, 01),       // DateOnly
                OfferedInEngland = true,                                     // True bool
                FundedInEngland = false,                                     // False bool
                GLH = 120,                                                   // Nullable int (Value)
                TQT = null                                                   // Nullable int (Null)
            };

            // Act
            var bytes = _sut.Build([row]);
            var csvContent = Encoding.UTF8.GetString(bytes);
            var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.True(lines.Length >= 2, "CSV should contain headers and at least one data row");

            // Verify Headers line
            Assert.StartsWith("QAN,QualificationTitle,AwardingOrganisation", lines[0]);

            // Verify explicit type formats inside the data line
            var dataCells = lines[1].Split(',');
            Assert.Equal("12345", dataCells[0]);
            Assert.Equal("Test Qualification", dataCells[1]);
            Assert.Equal("2026/05/20", dataCells[6]);   // OperationalEndDate formatted as yyyy-MM-dd
            Assert.Equal("True", dataCells[7]);         // OfferedInEngland
            Assert.Equal("False", dataCells[8]);        // FundedInEngland
            Assert.Equal("120", dataCells[9]);          // GLH
            Assert.Equal("", dataCells[10]);            // TQT (Null should map to blank)
            Assert.Equal("2026/06/01", dataCells[16]);  // FundingApprovalStartDate formatted as yyyy-MM-dd
        }

        [Theory]
        [InlineData("Standard Text", "Standard Text")]
        [InlineData("Value, with comma", "\"Value, with comma\"")]
        [InlineData("Value \"with\" quotes", "\"Value \"\"with\"\" quotes\"")]
        [InlineData("Value\nwith\rnewlines", "\"Value\nwith\rnewlines\"")]
        public void Build_ShouldEscapeWeirdCharacters_AccordingToCsvStandards(string weirdValue, string expectedEscapedOutput)
        {
            // Arrange
            var row = new RolloverWorkflowCandidatesExportRow
            {
                QAN = "111",
                QualificationTitle = weirdValue,
                Comments = weirdValue
            };

            // Act
            var bytes = _sut.Build([row]);
            var csvContent = Encoding.UTF8.GetString(bytes);

            var lines = csvContent.Split("\r\n");

            var targetRow = lines[1]; // row 0 = header, row 1 = data

            // Assert
            Assert.Contains(expectedEscapedOutput, targetRow);
        }


        [Fact]
        public void Build_ShouldHandleEmptyCollectionWithoutCrashing()
        {
            // Act
            var bytes = _sut.Build(Enumerable.Empty<RolloverWorkflowCandidatesExportRow>());
            var csvContent = Encoding.UTF8.GetString(bytes);
            var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.Single(lines); // Should only contain the header row
            Assert.StartsWith("QAN,QualificationTitle", lines[0]);
        }
    }
}

