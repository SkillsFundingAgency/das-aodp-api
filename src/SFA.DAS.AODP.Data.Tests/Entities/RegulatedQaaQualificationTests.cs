using Moq;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Entities;

public class RegulatedQaaQualificationTests : UnitTest
{
    private const string TestAimCode = "Z1234567";
    private const string TestQualificationTitle = "Access to Higher Education Diploma (Science)";
    private const string TestAwardingBody = "Test Awarding Body";
    private readonly DateTime _testSnapshot = new(2024, 02, 15);
    private readonly DateOnly _testStartDate = new(2023, 09, 01);
    private readonly DateOnly _testLastRegistrationDate = new(2025, 08, 31);
    private readonly SectorSubjectArea _testSectorSubjectArea = SectorSubjectArea.FromTiers("1", "1");

    [Fact]
    public void Create_WithValidParameters_ReturnsInstanceWithCorrectValuesAndDefaults()
    {
        // Act
        var qualification = RegulatedQaaQualification.Create(
            _testSnapshot,
            TestAimCode,
            TestQualificationTitle,
            TestAwardingBody,
            _testStartDate,
            _testLastRegistrationDate,
            _testSectorSubjectArea);

        // Assert
        qualification.DateOfDataSnapshot.ShouldBe(_testSnapshot);
        qualification.AimCode.ShouldBe(TestAimCode);
        qualification.QualificationTitle.ShouldBe(TestQualificationTitle);
        qualification.AwardingBody.ShouldBe(TestAwardingBody);
        qualification.StartDate.ShouldBe(_testStartDate);
        qualification.LastDateForRegistration.ShouldBe(_testLastRegistrationDate);
        qualification.SectorSubjectArea.ShouldBe(_testSectorSubjectArea);
        qualification.Level.ShouldBe("Level 3");
        qualification.Type.ShouldBe("Access to Higher Education");
        qualification.Status.ShouldBe("Approved");
        qualification.Age1619FundingApprovalEndDate.ShouldBeNull();
        qualification.AdvancedLearnerLoansFundingApprovalEndDate.ShouldBeNull();
        qualification.LegalEntitlementL2L3FundingApprovalEndDate.ShouldBeNull();
        qualification.Id.ShouldBe(Guid.Empty);
    }

    [Theory]
    [InlineData(QaaImportComparisonOutcome.LastDateForRegistrationChanged, QaaLastDateForRegistrationChangeType.BroughtForward)]
    [InlineData(QaaImportComparisonOutcome.LastDateForRegistrationChanged, QaaLastDateForRegistrationChangeType.Extended)]
    [InlineData(QaaImportComparisonOutcome.New, QaaLastDateForRegistrationChangeType.BroughtForward)]
    [InlineData(QaaImportComparisonOutcome.New, QaaLastDateForRegistrationChangeType.Extended)]
    public async Task SetFundingApprovalEndDate_WhenNeedToRecalculate_EnsureFundingApprovalCalculatorRunsAndSetsApprovalEndDates(QaaImportComparisonOutcome outcome, QaaLastDateForRegistrationChangeType changeType)
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 03, 10);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);

        qualification1.SetChangeOutcome(outcome, changeType);

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDateAsync(qualification1, FundingStream.Age1619, DateOnly.FromDateTime(publicationDate), CancellationToken)).ReturnsAsync(lastDateForRegistration);
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDateAsync(qualification1, FundingStream.AdvancedLearnerLoans, DateOnly.FromDateTime(publicationDate), CancellationToken)).ReturnsAsync(lastDateForRegistration);
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDateAsync(qualification1, FundingStream.LegalEntitlementL2L3, DateOnly.FromDateTime(publicationDate), CancellationToken)).ReturnsAsync(lastDateForRegistration);

        // Act
        await qualification1.SetFundingApprovalEndDateAsync(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object, CancellationToken);

        // Assert
        qualification1.Age1619FundingApprovalEndDate.ShouldBe(lastDateForRegistration);
        qualification1.AdvancedLearnerLoansFundingApprovalEndDate.ShouldBe(lastDateForRegistration);
        qualification1.LegalEntitlementL2L3FundingApprovalEndDate.ShouldBe(lastDateForRegistration);
    }

    [Theory]
    [InlineData(QaaImportComparisonOutcome.Unchanged, QaaLastDateForRegistrationChangeType.NotChanged)]
    [InlineData(QaaImportComparisonOutcome.Discontinued, QaaLastDateForRegistrationChangeType.NotChanged)]
    public async Task SetFundingApprovalEndDate_WhenDoNotNeedToRecalculate_EnsureFundingApprovalCalculatorDoesNotRun_FundingApprovalRemainsNull(QaaImportComparisonOutcome outcome, QaaLastDateForRegistrationChangeType changeType)
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 03, 10);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);

        qualification1.SetChangeOutcome(outcome, changeType);

        // Act
        await qualification1.SetFundingApprovalEndDateAsync(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object, CancellationToken);

        // Assert
        qualification1.Age1619FundingApprovalEndDate.ShouldBeNull();
        qualification1.AdvancedLearnerLoansFundingApprovalEndDate.ShouldBeNull();
        qualification1.LegalEntitlementL2L3FundingApprovalEndDate.ShouldBeNull();
    }
}