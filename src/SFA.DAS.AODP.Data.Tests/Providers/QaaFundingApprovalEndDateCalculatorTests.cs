using Moq;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Data.Repositories.Pldns;
using SFA.DAS.AODP.Testing.Stubs;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Providers;

public class QaaFundingApprovalEndDateCalculatorTests : UnitTest
{
    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_LastDateForRegistrationInCurrentAcademicYear_ReturnsEndOfCurrentAcademicYear()
    {
        // Arrange
        var mockAcademicYearProvider = new Mock<IAcademicYearProvider>();
        var qan = "QAN-001";
        var lastDateForRegistration = new DateOnly(2026, 6, 5);
        var publicationDate = new DateOnly(2026, 6, 12);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var pldnsRepository = CreatePldnsRepositoryReturning(new DateTime(2028, 01, 02), new DateTime(2028, 01, 02), new DateTime(2028, 01, 02));
        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Expectations
        mockAcademicYearProvider.Setup(o => o.IsWithinCurrentAcademicYear(new DateTime(2028, 01, 02))).Returns(false);
        mockAcademicYearProvider.Setup(o => o.GetCurrentAcademicYearEndDate()).Returns(new DateOnly(2026, 07, 31));
        mockAcademicYearProvider.Setup(o => o.GetAcademicYearEndForDate(lastDateForRegistration)).Returns(new DateOnly(2026, 7, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            mockAcademicYearProvider.Object,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_PldnsIsInFutureAcademicYear_LastDateForRegistrationAfterPublicationDate_WithinCurrentAcademicYear_ReturnEndOfCurrentAcademicYear()
    {
        // Arrange
        var mockAcademicYearProvider = new Mock<IAcademicYearProvider>();
        var qan = "QAN-001";
        var lastDateForRegistration = new DateOnly(2026, 6, 30);
        var publicationDate = new DateOnly(2026, 6, 12);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var pldnsRepository = CreatePldnsRepositoryReturning(new DateTime(2028, 01, 02), new DateTime(2028, 01, 02), new DateTime(2028, 01, 02));
        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Expectations
        mockAcademicYearProvider.Setup(o => o.IsWithinCurrentAcademicYear(new DateTime(2028, 01, 02))).Returns(false);
        mockAcademicYearProvider.Setup(o => o.GetCurrentAcademicYearEndDate()).Returns(new DateOnly(2026, 07, 31));
        mockAcademicYearProvider.Setup(o => o.GetAcademicYearEndForDate(lastDateForRegistration)).Returns(new DateOnly(2026, 7, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            mockAcademicYearProvider.Object,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndBeforeIlrDeadline_AndNotInCurrentAcademicYear_ReturnEndOfNextAcademicYear()
    {
        // Arrange
        var qan = "QAN-001";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var publicationDate = new DateOnly(2026, 6, 12);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndOnOrAfterIlrDeadline_NoPldns_UsesAcademicYearOfTheFollowingYear()
    {
        // Arrange
        var qan = "QAN-002";
        var lastDateForRegistration = new DateOnly(2027, 9, 30);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 10,23);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 22));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();
        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2028, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndOnOrAfterIlrDeadline_PldnsSetButInFutureAcademicYear_UsesAcademicYearOfTheFollowingYear()
    {
        // Arrange
        var mockAcademicYearProvider = new Mock<IAcademicYearProvider>();
        var qan = "QAN-002";
        var lastDateForRegistration = new DateOnly(2027, 9, 30);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 10, 23);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 22));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: new DateTime(2029, 07, 31), 
            pldns16To19: new DateTime(2029, 07, 31),
            legalEntitlementL2L3: new DateTime(2029, 07, 31));

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Expectations
        mockAcademicYearProvider.Setup(o => o.GetCurrentAcademicYearEndDate()).Returns(new DateOnly(2026, 7, 31));
        mockAcademicYearProvider.Setup(o => o.AreDatesWithinSameAcademicYear(new DateTime(2029, 07, 31), new DateOnly(2028, 07, 31))).Returns(false);
        mockAcademicYearProvider.Setup(o => o.GetAcademicYearEndForDate(new DateOnly(2027, 9, 30))).Returns(new DateOnly(2028, 7, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            mockAcademicYearProvider.Object,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2028, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndLastDateIsBeforeAcademicYear_ReturnsEndOfCurrentAcademicYear()
    {
        // Arrange
        var qan = "QAN-003";
        var lastDateForRegistration = new DateOnly(2026, 6, 30);
        var publicationDate = new DateOnly(2026, 6, 1);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 5, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();
        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 07, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsInCurrentAcademicYear_ReturnsEndOfCurrentAcademicYear()
    {
        // Arrange
        var qan = "QAN-004";
        var lastDateForRegistration = new DateOnly(2026, 5, 31);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsInCurrentAcademicYear_IgnoresExistingFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-005";
        var lastDateForRegistration = new DateOnly(2026, 5, 31);
        var currentFundingApprovalEndDate = new DateOnly(2026, 4, 30);
        var publicationDate = new DateOnly(2026, 6, 12);
        
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance, currentFundingApprovalEndDate);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsInCurrentAcademicYear_RecalculatesToAcademicYearEnd()
    {
        // Arrange
        var qan = "QAN-006";
        var lastDateForRegistration = new DateOnly(2026, 3, 31);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 6, 12);
        
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance, currentFundingApprovalEndDate);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(currentFundingApprovalEndDate);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationEqualsPublicationDate_AndIsInCurrentAcademicYear_ReturnsAcademicYearEnd()
    {
        // Arrange
        var qan = "QAN-007";
        var lastDateForRegistration = new DateOnly(2026, 6, 12);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance, currentFundingApprovalEndDate);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(currentFundingApprovalEndDate);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenPldnsHasEarlierDate_ReturnsEarliestPldnsDate()
    {
        // Arrange
        var qan = "QAN-008";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: new DateTime(2026, 6, 30),
            pldns16To19: new DateTime(2026, 5, 31),
            legalEntitlementL2L3: new DateTime(2026, 7, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 10, 15)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 5, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenPldnsComesSoonerThanEndOfAcademicYear_AndLastDateForRegistrationInCurrentAcademicYear_ReturnPldnsDate()
    {
        // Arrange
        var qan = "QAN-009";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: new DateTime(2026, 8, 31),
            pldns16To19: new DateTime(2026, 9, 20),
            legalEntitlementL2L3: new DateTime(2026, 10, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 10, 15)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 9, 20));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenPldnsDatesAreAllNull_DoesNotChangeCalculatedFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-010";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: null,
            pldns16To19: null,
            legalEntitlementL2L3: null);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 10, 15)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2027, 7, 31)),
            pldnsRepository.Object);

        var qualification = RegulatedQaaQualification.Create(new DateTime(2026, 01, 02), qan, "title", "ao", new DateOnly(2025, 09, 01), lastDateForRegistration, SectorSubjectArea.AccountingAndFinance);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_LastDateForRegistrationInPreviousAcademicYear_ReturnsPublicationDate()
    {
        var publicationDate = new DateOnly(2026, 6, 12);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-011",
            "title",
            "ao",
            new DateOnly(2024, 9, 1),
            new DateOnly(2025, 7, 31),
            SectorSubjectArea.AccountingAndFinance,
            new DateOnly(2027, 7, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            CreatePldnsRepositoryReturningNull().Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(publicationDate);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_PublicationOnIlrDeadline_DoesNotExtendByAnAdditionalAcademicYear()
    {
        var publicationDate = new DateOnly(2026, 10, 22);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-012",
            "title",
            "ao",
            new DateOnly(2025, 9, 1),
            new DateOnly(2027, 9, 30),
            SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", publicationDate)),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            CreatePldnsRepositoryReturningNull().Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_PublicationAfterIlrDeadline_DoesNotExtendPastRegistrationAcademicYear()
    {
        var publicationDate = new DateOnly(2026, 10, 23);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-015",
            "title",
            "ao",
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 9, 30),
            SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            CreatePldnsRepositoryReturningNull().Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_FutureRegistration_PldnsInEarlierAcademicYear_ReturnsPldns()
    {
        var publicationDate = new DateOnly(2026, 6, 12);
        var pldnsDate = new DateTime(2026, 5, 31);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-013",
            "title",
            "ao",
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 9, 30),
            SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            CreatePldnsRepositoryReturning(pldnsDate, pldnsDate, pldnsDate).Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(DateOnly.FromDateTime(pldnsDate));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_PldnsBeforeCalculatedEndDate_InDifferentAcademicYear_ReturnsPldns()
    {
        var publicationDate = new DateOnly(2026, 9, 15);
        var pldnsDate = new DateTime(2026, 7, 31);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-016",
            "title",
            "ao",
            new DateOnly(2024, 9, 1),
            new DateOnly(2025, 7, 31),
            SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2027, 7, 31)),
            CreatePldnsRepositoryReturning(pldnsDate, pldnsDate, pldnsDate).Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(DateOnly.FromDateTime(pldnsDate));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_FutureRegistration_PldnsAfterCalculatedEndDate_KeepsCalculatedEndDate()
    {
        var publicationDate = new DateOnly(2026, 6, 12);
        var pldnsDate = new DateTime(2027, 8, 1);
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 2),
            "QAN-014",
            "title",
            "ao",
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 9, 30),
            SectorSubjectArea.AccountingAndFinance);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(publicationDate),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            CreatePldnsRepositoryReturning(pldnsDate, pldnsDate, pldnsDate).Object);

        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qualification,
            FundingStream.Age1619,
            publicationDate,
            CancellationToken);

        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    private static Mock<IPldnsRepository> CreatePldnsRepositoryReturningNull()
    {
        var pldnsRepository = new Mock<IPldnsRepository>(MockBehavior.Strict);

        pldnsRepository
            .Setup(repository => repository.GetPldnsByQanAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pldns?)null!);

        return pldnsRepository;
    }

    private static Mock<IPldnsRepository> CreatePldnsRepositoryReturning(DateTime? loans, DateTime? pldns16To19, DateTime? legalEntitlementL2L3)
    {
        var pldnsRepository = new Mock<IPldnsRepository>(MockBehavior.Strict);

        pldnsRepository
            .Setup(repository => repository.GetPldnsByQanAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pldns
            {
                Loans = loans,
                Pldns16To19 = pldns16To19,
                LegalEntitlementL2L3 = legalEntitlementL2L3
            });

        return pldnsRepository;
    }

    private sealed class FakeIlrSubmissionDeadlinesProvider(IlrSubmissionDeadline finalSubmissionDeadline) : IIlrSubmissionDeadlinesProvider
    {
        public IlrSubmissionDeadline GetFinalSubmissionDeadline()
        {
            return finalSubmissionDeadline;
        }
    }

    private sealed class FakeAcademicYearProvider(DateOnly currentAcademicYearEndDate) : IAcademicYearProvider
    {
        public DateOnly GetCurrentAcademicYearEndDate()
        {
            return currentAcademicYearEndDate;
        }

        public DateOnly GetAcademicYearEndForDate(DateOnly dateOnly)
        {
            return dateOnly > new DateOnly(dateOnly.Year, 7, 31)
                ? new DateOnly(dateOnly.Year + 1, 7, 31)
                : new DateOnly(dateOnly.Year, 7, 31);
        }

        public bool IsWithinCurrentAcademicYear(DateTime? dateToCheck)
        {
            return dateToCheck is not null
                && GetAcademicYearEndForDate(DateOnly.FromDateTime(dateToCheck.Value)) == currentAcademicYearEndDate;
        }

        public bool AreDatesWithinSameAcademicYear(DateTime? firstDateToCheck, DateOnly? secondDateToCheck)
        {
            return firstDateToCheck is not null
                && secondDateToCheck is not null
                && GetAcademicYearEndForDate(DateOnly.FromDateTime(firstDateToCheck.Value)) == GetAcademicYearEndForDate(secondDateToCheck.Value);
        }
    }
}
