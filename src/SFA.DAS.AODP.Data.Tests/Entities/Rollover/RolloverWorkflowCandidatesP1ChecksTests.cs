using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.UnitTests.Entities.Rollover;

public class RolloverWorkflowCandidatesP1ChecksTests
{
    [Theory]
    [MemberData(nameof(GetPldnsDateCases))]
    public void GetPldnsDate_ReturnsExpectedDate_ForKnownFundingStreams(
        string fundingStream,
        DateTime? expectedDate)
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = fundingStream,
            Age1416 = new DateTime(2024, 1, 1),
            Age1619 = new DateTime(2024, 2, 1),
            LocalFlexibilities = new DateTime(2024, 3, 1),
            LegalEntitlementL2L3 = new DateTime(2024, 4, 1),
            LegalEntitlementEnglishandMaths = new DateTime(2024, 5, 1),
            DigitalEntitlement = new DateTime(2024, 6, 1),
            ESFL3L4 = new DateTime(2024, 7, 1),
            AdvancedLearnerLoans = new DateTime(2024, 8, 1),
            LifelongLearningEntitlement = new DateTime(2024, 9, 1),
            L3FreeCoursesForJobs = new DateTime(2024, 10, 1),
            CoF = new DateTime(2024, 11, 1)
        };

        // Act
        var result = sut.GetPldnsDate();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    public static IEnumerable<object[]> GetPldnsDateCases()
    {
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.Age1416), new DateTime(2024, 1, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.Age1619), new DateTime(2024, 2, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.LocalFlexibilities), new DateTime(2024, 3, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.LegalEntitlementL2L3), new DateTime(2024, 4, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.LegalEntitlementEnglishandMaths), new DateTime(2024, 5, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.DigitalEntitlement), new DateTime(2024, 6, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.ESFL3L4), new DateTime(2024, 7, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.AdvancedLearnerLoans), new DateTime(2024, 8, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.LifelongLearningEntitlement), new DateTime(2024, 9, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.L3FreeCoursesForJobs), new DateTime(2024, 10, 1)];
        yield return [nameof(RolloverWorkflowCandidatesP1Checks.CoF), new DateTime(2024, 11, 1)];
    }

    [Fact]
    public void GetPldnsDate_ReturnsNull_WhenFundingStreamIsNullOrWhitespace()
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = "   "
        };

        // Act
        var result = sut.GetPldnsDate();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPldnsDate_ReturnsNull_WhenFundingStreamIsUnknown()
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            FundingStream = "UnknownFundingStream"
        };

        // Act
        var result = sut.GetPldnsDate();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAcademicYearDates_ReturnsDefaultValues_WhenAcademicYearIsNullOrWhitespace()
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            AcademicYear = "   "
        };

        // Act
        var result = sut.GetAcademicYearDates();

        // Assert
        Assert.Null(result.AcademicYearStartDate);
        Assert.Null(result.AcademicYearEndDate);
    }

    [Theory]
    [InlineData("2024")]
    [InlineData("abcd/25")]
    public void GetAcademicYearDates_ThrowsArgumentException_WhenAcademicYearIsInvalid(string academicYear)
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            AcademicYear = academicYear
        };

        // Act
        var exception = Assert.Throws<ArgumentException>(() => sut.GetAcademicYearDates());

        // Assert
        Assert.Equal("AcademicYear", exception.ParamName);
    }

    [Fact]
    public void GetAcademicYearDates_ReturnsUtcAcademicYearDates_WhenAcademicYearIsValid()
    {
        // Arrange
        var sut = new RolloverWorkflowCandidatesP1Checks
        {
            AcademicYear = "2024/25"
        };

        // Act
        var result = sut.GetAcademicYearDates();

        // Assert
        Assert.Equal(new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc), result.AcademicYearStartDate);
        Assert.Equal(new DateTime(2025, 7, 31, 0, 0, 0, DateTimeKind.Utc), result.AcademicYearEndDate);
        Assert.Equal(DateTimeKind.Utc, result.AcademicYearStartDate!.Value.Kind);
        Assert.Equal(DateTimeKind.Utc, result.AcademicYearEndDate!.Value.Kind);
    }
}
