using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QueryExtensions;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Entities.QueryExtensions;

public class QualificationVersionQueryExtensionsTests
{
    [Fact]
    public void WhereEligibleForFunding_ShouldOnlyReturnQualificationVersionsWhereEligibleForFundingIsTrue()
    {
        // Arrange
        var qualificationVersions = new List<QualificationVersions>
        {
            CreateQualificationVersion(qualificationId: Guid.NewGuid(), version: 1, eligibleForFunding: true),
            CreateQualificationVersion(qualificationId: Guid.NewGuid(), version: 1, eligibleForFunding: false),
            CreateQualificationVersion(qualificationId: Guid.NewGuid(), version: 1, eligibleForFunding: true)
        }.AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereEligibleForFunding()
            .ToList();

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldAllBe(qv => qv.EligibleForFunding == true);
    }

    [Fact]
    public void WhereEligibleForFunding_ShouldReturnEmptyCollection_WhenNoQualificationVersionsAreEligibleForFunding()
    {
        // Arrange
        var qualificationVersions = new List<QualificationVersions>
        {
            CreateQualificationVersion(qualificationId: Guid.NewGuid(), version: 1, eligibleForFunding: false),
            CreateQualificationVersion(qualificationId: Guid.NewGuid(), version: 2, eligibleForFunding: false)
        }.AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereEligibleForFunding()
            .ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereLatestVersionPerQualification_ShouldReturnLatestVersionForEachQualification()
    {
        // Arrange
        var qualificationId1 = Guid.NewGuid();
        var qualificationId2 = Guid.NewGuid();

        var qualificationVersions = new List<QualificationVersions>
        {
            CreateQualificationVersion(qualificationId1, version: 1),
            CreateQualificationVersion(qualificationId1, version: 2),
            CreateQualificationVersion(qualificationId1, version: 3),

            CreateQualificationVersion(qualificationId2, version: 1),
            CreateQualificationVersion(qualificationId2, version: 4),
            CreateQualificationVersion(qualificationId2, version: 2)
        }.AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereLatestVersionPerQualification()
            .ToList();

        // Assert
        result.Count.ShouldBe(2);

        var resultByQualificationId = result.ToDictionary(qv => qv.QualificationId);

        resultByQualificationId[qualificationId1].Version.ShouldBe(3);
        resultByQualificationId[qualificationId2].Version.ShouldBe(4);
    }

    [Fact]
    public void WhereLatestVersionPerQualification_ShouldReturnSingleVersion_WhenQualificationOnlyHasOneVersion()
    {
        // Arrange
        var qualificationId = Guid.NewGuid();

        var qualificationVersions = new List<QualificationVersions>
        {
            CreateQualificationVersion(qualificationId, version: 1)
        }.AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereLatestVersionPerQualification()
            .ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationId.ShouldBe(qualificationId);
        result.Single().Version.ShouldBe(1);
    }

    [Fact]
    public void WhereLatestVersionPerQualification_ShouldReturnEmptyCollection_WhenSourceQueryIsEmpty()
    {
        // Arrange
        var qualificationVersions = new List<QualificationVersions>()
            .AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereLatestVersionPerQualification()
            .ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereLatestVersionPerQualification_ShouldNotMixVersionsBetweenDifferentQualifications()
    {
        // Arrange
        var qualificationId1 = Guid.NewGuid();
        var qualificationId2 = Guid.NewGuid();

        var qualificationVersions = new List<QualificationVersions>
        {
            CreateQualificationVersion(qualificationId1, version: 10),
            CreateQualificationVersion(qualificationId2, version: 20),
            CreateQualificationVersion(qualificationId1, version: 30),
            CreateQualificationVersion(qualificationId2, version: 5)
        }.AsQueryable();

        // Act
        var result = qualificationVersions
            .WhereLatestVersionPerQualification()
            .ToList();

        // Assert
        result.Count.ShouldBe(2);

        result.Single(qv => qv.QualificationId == qualificationId1).Version.ShouldBe(30);
        result.Single(qv => qv.QualificationId == qualificationId2).Version.ShouldBe(20);
    }

    private static QualificationVersions CreateQualificationVersion(
        Guid qualificationId,
        int version,
        bool eligibleForFunding = false)
    {
        return new QualificationVersions
        {
            QualificationId = qualificationId,
            Version = version,
            EligibleForFunding = eligibleForFunding
        };
    }
}