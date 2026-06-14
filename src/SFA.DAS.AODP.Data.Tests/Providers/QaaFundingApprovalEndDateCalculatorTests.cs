using Moq;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Data.Repositories.Pldns;
using SFA.DAS.AODP.Testing.Stubs;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Providers;

public class QaaFundingApprovalEndDateCalculatorTests : UnitTest
{
    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndBeforeIlrDeadline_ReturnsEarlierOfLastDateForRegistrationAndCurrentAcademicYear()
    {
        // Arrange
        var qan = "QAN-001";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var currentFundingApprovalEndDate = (DateOnly?)null;
        var publicationDate = new DateOnly(2026, 6, 12);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndOnOrAfterIlrDeadline_UsesNextAcademicYear()
    {
        // Arrange
        var qan = "QAN-002";
        var lastDateForRegistration = new DateOnly(2027, 9, 30);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 6, 12);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 22));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2027, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsAfterPublicationDate_AndLastDateIsBeforeAcademicYear_ReturnsLastDateForRegistration()
    {
        // Arrange
        var qan = "QAN-003";
        var lastDateForRegistration = new DateOnly(2026, 6, 30);
        var currentFundingApprovalEndDate = (DateOnly?)null;
        var publicationDate = new DateOnly(2026, 6, 1);

        var clockProvider = new FakeSystemClockProvider(new DateOnly(2026, 10, 15));
        var ilrSubmissionDeadlinesProvider = new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22)));
        var academicYearProvider = new FakeAcademicYearProvider(new DateOnly(2026, 7, 31));
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            clockProvider,
            ilrSubmissionDeadlinesProvider,
            academicYearProvider,
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(lastDateForRegistration);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsBeforePublicationDate_AndCurrentFundingApprovalEndDateIsNull_ReturnsLastDateForRegistration()
    {
        // Arrange
        var qan = "QAN-004";
        var lastDateForRegistration = new DateOnly(2026, 5, 31);
        DateOnly? currentFundingApprovalEndDate = null;
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(lastDateForRegistration);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsBeforePublicationDate_AndAfterCurrentFundingApprovalEndDate_ReturnsLastDateForRegistration()
    {
        // Arrange
        var qan = "QAN-005";
        var lastDateForRegistration = new DateOnly(2026, 5, 31);
        var currentFundingApprovalEndDate = new DateOnly(2026, 4, 30);
        var publicationDate = new DateOnly(2026, 6, 12);
        
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(lastDateForRegistration);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationIsBeforePublicationDate_AndBeforeCurrentFundingApprovalEndDate_KeepsCurrentFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-006";
        var lastDateForRegistration = new DateOnly(2026, 3, 31);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 6, 12);
        
        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(currentFundingApprovalEndDate);
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenLastDateForRegistrationEqualsPublicationDate_KeepsCurrentFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-007";
        var lastDateForRegistration = new DateOnly(2026, 6, 12);
        var currentFundingApprovalEndDate = new DateOnly(2026, 7, 31);
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
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
        var currentFundingApprovalEndDate = (DateOnly?)null;
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

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 5, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenPldnsDatesAreLaterThanCalculatedDate_KeepsCalculatedFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-009";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var currentFundingApprovalEndDate = (DateOnly?)null;
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: new DateTime(2026, 8, 31),
            pldns16To19: new DateTime(2026, 9, 30),
            legalEntitlementL2L3: new DateTime(2026, 10, 31));

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 10, 15)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_WhenPldnsDatesAreAllNull_DoesNotChangeCalculatedFundingApprovalEndDate()
    {
        // Arrange
        var qan = "QAN-010";
        var lastDateForRegistration = new DateOnly(2026, 9, 30);
        var currentFundingApprovalEndDate = (DateOnly?)null;
        var publicationDate = new DateOnly(2026, 6, 12);

        var pldnsRepository = CreatePldnsRepositoryReturning(
            loans: null,
            pldns16To19: null,
            legalEntitlementL2L3: null);

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 10, 15)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        var result = await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            lastDateForRegistration,
            currentFundingApprovalEndDate,
            publicationDate,
            CancellationToken);

        // Assert
        result.ShouldBe(new DateOnly(2026, 7, 31));
    }

    [Fact]
    public async Task CalculateFundingApprovalEndDateAsync_PassesQanAndCancellationTokenToPldnsRepository()
    {
        // Arrange
        var qan = "QAN-011";

        var pldnsRepository = CreatePldnsRepositoryReturningNull();

        var sut = new QaaFundingApprovalEndDateCalculator(
            new FakeSystemClockProvider(new DateOnly(2026, 6, 12)),
            new FakeIlrSubmissionDeadlinesProvider(new IlrSubmissionDeadline("R14", new DateOnly(2026, 10, 22))),
            new FakeAcademicYearProvider(new DateOnly(2026, 7, 31)),
            pldnsRepository.Object);

        // Act
        await sut.CalculateFundingApprovalEndDateAsync(
            qan,
            new DateOnly(2026, 6, 12),
            new DateOnly(2026, 7, 31),
            new DateOnly(2026, 6, 12),
            CancellationToken);

        // Assert
        pldnsRepository.Verify(
            repository => repository.GetPldnsByQanAsync(qan, CancellationToken),
            Times.Once);
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

        public bool IsWithinCurrentAcademicYear(DateTime? dateToCheck) => true;
    }
}