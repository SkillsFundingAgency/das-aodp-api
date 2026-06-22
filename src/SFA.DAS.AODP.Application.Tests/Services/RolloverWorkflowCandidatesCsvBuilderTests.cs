using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Models.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Services
{
    public class FundingExtensionCandidatesCsvBuilderTests
    {
        private readonly FundingExtensionCandidatesCsvBuilder _sut;

        public FundingExtensionCandidatesCsvBuilderTests()
        {
            _sut = new FundingExtensionCandidatesCsvBuilder();
        }


        [Fact]
        public void BuildWithValidationErrors_ShouldIncludeValidationColumn_AndErrors()
        {
            // Arrange
            var row = new RolloverCandidateForExport
            {
                RowNumber = 1,
                QAN = "123",
                QualificationTitle = "Test",
                Comments = "None"
            };

            var validation = new CandidateValidationResult
            {
                CandidateDetails = new RolloverCandidateForValidation { RowNumber = 1 },
                Errors =
                {
                    new ValidationFailure { Message = "Error A" },
                    new ValidationFailure { Message = "Error B" }
                }
            };

            // Act
            var csv = Encoding.UTF8.GetString(
                _sut.BuildWithValidationErrors([row], [validation]));

            // Assert
            Assert.Contains("ValidationErrors", csv);
            Assert.Contains("Error A; Error B", csv);
        }

        [Fact]
        public void BuildWithValidationErrors_ShouldWriteEmptyValidationColumn_WhenNoErrors()
        {
            // Arrange
            var row = new RolloverCandidateForExport
            {
                RowNumber = 2,
                QAN = "999",
                QualificationTitle = "No Errors"
            };

            var validation = new CandidateValidationResult
            {
                CandidateDetails = new RolloverCandidateForValidation { RowNumber = 2 }
            };

            // Act
            var csv = Encoding.UTF8.GetString(
                _sut.BuildWithValidationErrors([row], [validation]));

            // Assert
            Assert.Contains("ValidationErrors", csv);
            Assert.Contains("No Errors", csv);
            Assert.Contains(",\r\n", csv); // empty validation column
        }

        [Fact]
        public void BuildWithValidationErrors_ShouldHandleMissingValidationResult()
        {
            // Arrange
            var row = new RolloverCandidateForExport
            {
                RowNumber = 10,
                QAN = "ABC",
                QualificationTitle = "Missing Validation"
            };

            // Act
            var csv = Encoding.UTF8.GetString(
                _sut.BuildWithValidationErrors([row], Enumerable.Empty<CandidateValidationResult>()));

            // Assert
            Assert.Contains("ValidationErrors", csv);
            Assert.Contains(",\r\n", csv); // empty validation column
        }


        [Fact]
        public void Build_ShouldWriteBooleanValues()
        {
            var row = new RolloverCandidateForExport
            {
                QAN = "1",
                QualificationTitle = "Bool Test",
                OfferedInEngland = true,
                FundedInEngland = false
            };

            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            Assert.Contains("True", csv);
            Assert.Contains("False", csv);
        }

        [Fact]
        public void Build_ShouldWriteDateTimeInCsvFormat()
        {
            var row = new RolloverCandidateForExport
            {
                QAN = "1",
                QualificationTitle = "DateTime Test",
                OperationalEndDate = new DateTime(2025, 12, 25)
            };

            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            Assert.Contains("2025/12/25", csv);
        }

        [Fact]
        public void Build_ShouldWriteDateOnlyInCsvFormat()
        {
            var row = new RolloverCandidateForExport
            {
                QAN = "1",
                QualificationTitle = "DateOnly Test",
                FundingApprovalStartDate = new DateOnly(2026, 1, 1)
            };

            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            Assert.Contains("2026/01/01", csv);
        }

        [Theory]
        [InlineData("Hello", "Hello")]
        [InlineData("Value,WithComma", "\"Value,WithComma\"")]
        [InlineData("Value \"With\" Quotes", "\"Value \"\"With\"\" Quotes\"")]
        [InlineData("Line1\nLine2", "\"Line1\nLine2\"")]
        public void Build_ShouldEscapeStringsCorrectly(string input, string expected)
        {
            var row = new RolloverCandidateForExport
            {
                QAN = "1",
                QualificationTitle = input,
                Comments = input
            };

            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            Assert.Contains(expected, csv);
        }

        [Fact]
        public void Build_ShouldHandleNullFields()
        {
            var row = new RolloverCandidateForExport
            {
                QAN = "1",
                QualificationTitle = null,
                Comments = null
            };

            var csv = Encoding.UTF8.GetString(_sut.Build([row]));

            // Should not throw and should contain empty fields
            Assert.Contains("1,", csv);
        }
    }
}
