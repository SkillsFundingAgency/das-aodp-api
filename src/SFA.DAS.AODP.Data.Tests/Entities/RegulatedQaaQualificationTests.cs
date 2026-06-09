using Moq;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
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
        qualification.LastFundingApprovalEndDate.ShouldBeNull();
        qualification.Id.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithVaryingInputs_MapsPropertiesCorrectly()
    {
        // Arrange
        var qualifications = new[]
        {
            ("Z1234567", "Diploma 1", "Body 1"),
            ("Z7654321", "Diploma 2", "Body 2"),
            ("Z1111111", "Diploma 3", "Body 3")
        };

        // Act
        var createdQualifications = qualifications
            .Select(q => RegulatedQaaQualification.Create(
                _testSnapshot,
                q.Item1,
                q.Item2,
                q.Item3,
                _testStartDate,
                _testLastRegistrationDate,
                _testSectorSubjectArea))
            .ToList();

        // Assert
        createdQualifications.Count.ShouldBe(3);
        createdQualifications[0].AimCode.ShouldBe("Z1234567");
        createdQualifications[0].QualificationTitle.ShouldBe("Diploma 1");
        createdQualifications[0].AwardingBody.ShouldBe("Body 1");

        createdQualifications[1].AimCode.ShouldBe("Z7654321");
        createdQualifications[1].QualificationTitle.ShouldBe("Diploma 2");
        createdQualifications[1].AwardingBody.ShouldBe("Body 2");

        createdQualifications[2].AimCode.ShouldBe("Z1111111");
        createdQualifications[2].QualificationTitle.ShouldBe("Diploma 3");
        createdQualifications[2].AwardingBody.ShouldBe("Body 3");
    }

    [Fact]
    public void Create_WithDifferentSnapshots_EachInstancePreservesCorrectDate()
    {
        // Arrange
        var snapshot1 = new DateTime(2024, 02, 15);
        var snapshot2 = new DateTime(2024, 03, 15);
        var snapshot3 = new DateTime(2024, 04, 15);

        // Act
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, _testLastRegistrationDate, _testSectorSubjectArea);

        var qualification2 = RegulatedQaaQualification.Create(
            snapshot2, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, _testLastRegistrationDate, _testSectorSubjectArea);

        var qualification3 = RegulatedQaaQualification.Create(
            snapshot3, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, _testLastRegistrationDate, _testSectorSubjectArea);

        // Assert
        qualification1.DateOfDataSnapshot.ShouldBe(snapshot1);
        qualification2.DateOfDataSnapshot.ShouldBe(snapshot2);
        qualification3.DateOfDataSnapshot.ShouldBe(snapshot3);
    }

    [Fact]
    public void SetFundingApprovalEndDate_LastDateForRegistrationAfterPublicationDate_LastDateEarlierThanAcademicYearEnd_ShouldUseLastDateForRegistration()
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 03, 10);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDate(lastDateForRegistration, qualification1.LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate))).Returns(lastDateForRegistration);

        // Act
        qualification1.SetFundingApprovalEndDate(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object);

        // Assert
        qualification1.LastFundingApprovalEndDate.ShouldBe(lastDateForRegistration);
    }

    [Fact]
    public void SetFundingApprovalEndDate_LastDateForRegistrationAfterPublicationDate_LastDateLaterThanAcademicYearEnd_ShouldUseAcademicYearEnd()
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var academicYearEndDate = new DateOnly(2026, 07, 31);
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 03, 10);
        var lastDateForRegistration = new DateOnly(2026, 08, 1);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);
        qualification1.LastFundingApprovalEndDate = new DateOnly(2026, 03, 20);

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDate(lastDateForRegistration, qualification1.LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate))).Returns(academicYearEndDate);

        // Act
        qualification1.SetFundingApprovalEndDate(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object);

        // Assert
        qualification1.LastFundingApprovalEndDate.ShouldBe(academicYearEndDate);
    }

    [Fact]
    public void SetFundingApprovalEndDate_LastDateForRegistrationBeforePublicationDate_LastDateAfterCurrentApproval_UseLastDate()
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 04, 30);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);
        qualification1.LastFundingApprovalEndDate = new DateOnly(2026, 03, 20);

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDate(lastDateForRegistration, qualification1.LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate))).Returns(lastDateForRegistration);

        // Act
        qualification1.SetFundingApprovalEndDate(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object);

        // Assert
        qualification1.LastFundingApprovalEndDate.ShouldBe(lastDateForRegistration);
    }

    [Fact]
    public void SetFundingApprovalEndDate_LastDateForRegistrationBeforePublicationDate_LastDateBeforeCurrentApproval_UseExistingValue()
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 04, 30);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);
        qualification1.LastFundingApprovalEndDate = new DateOnly(2026, 04, 20);

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDate(lastDateForRegistration, qualification1.LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate))).Returns(new DateOnly(2026, 04, 20));

        // Act
        qualification1.SetFundingApprovalEndDate(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object);

        // Assert
        qualification1.LastFundingApprovalEndDate.ShouldBe(new DateOnly(2026, 04, 20));
    }

    [Fact]
    public void SetFundingApprovalEndDate_LastDateForRegistrationBeforePublicationDate_CurrentApprovalNull_UseLastDate()
    {
        // Arrange
        var mockQaaFundingApprovalEndDateCalculator = new Mock<IQaaFundingApprovalEndDateCalculator>();
        var snapshot1 = new DateTime(2024, 02, 15);
        var publicationDate = new DateTime(2026, 04, 30);
        var lastDateForRegistration = new DateOnly(2026, 03, 30);
        var qualification1 = RegulatedQaaQualification.Create(
            snapshot1, TestAimCode, TestQualificationTitle, TestAwardingBody,
            _testStartDate, lastDateForRegistration, _testSectorSubjectArea);
        qualification1.LastFundingApprovalEndDate = null;

        // Expectations
        mockQaaFundingApprovalEndDateCalculator.Setup(o => o.CalculateFundingApprovalEndDate(lastDateForRegistration, qualification1.LastFundingApprovalEndDate, DateOnly.FromDateTime(publicationDate))).Returns(lastDateForRegistration);

        // Act
        qualification1.SetFundingApprovalEndDate(publicationDate, mockQaaFundingApprovalEndDateCalculator.Object);

        // Assert
        qualification1.LastFundingApprovalEndDate.ShouldBe(new DateOnly(2026, 03, 30));
    }
}