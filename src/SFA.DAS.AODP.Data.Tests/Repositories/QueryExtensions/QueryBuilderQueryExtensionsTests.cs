using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.QueryExtensions;
using SFA.DAS.AODP.Data.ValueObjects;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.QueryExtensions;

public class RolloverQueryBuilderQueryExtensionsTests
{
    [Fact]
    public void WithLevelFilter_Should_Return_Only_Matching_Levels()
    {
        // Arrange
        var targetLevel = QualificationLevel.Level1.ToString();

        var qualifications = new[]
        {
            new QualificationVersions { Level = targetLevel },
            new QualificationVersions { Level = QualificationLevel.Level2.ToString() }
        }.AsQueryable();

        var levelIds = new[]
        {
            QualificationLevel.Level1.Id
        };

        // Act
        var result = qualifications.WithLevelFilter(levelIds).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().Level.ShouldBe(targetLevel);
    }

    [Fact]
    public void WithTypeFilter_Should_Return_Only_Matching_Types()
    {
        // Arrange
        var targetType = QualificationType.AccessToHigherEducation.ToString();

        var qualifications = new[]
        {
            new QualificationVersions { Type = targetType },
            new QualificationVersions { Type = QualificationType.AdvancedExtensionAward.ToString() }
        }.AsQueryable();

        var typeIds = new[]
        {
            QualificationType.AccessToHigherEducation.Id
        };

        // Act
        var result = qualifications.WithTypeFilter(typeIds).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().Type.ShouldBe(targetType);
    }

    [Fact]
    public void WithSectorSubjectAreaFilter_Should_Return_Only_Matching_Ssa()
    {
        // Arrange
        var targetSsa = SectorSubjectArea.FromFullCode("1.1").ToString();

        var qualifications = new[]
        {
            new QualificationVersions { Ssa = targetSsa },
            new QualificationVersions { Ssa = SectorSubjectArea.FromFullCode("2.1").ToString() }
        }.AsQueryable();

        var ssaCodes = new[] { "1.1" };

        // Act
        var result = qualifications.WithSectorSubjectAreaFilter(ssaCodes).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().Ssa.ShouldBe(targetSsa);
    }

    [Fact]
    public void WithAwardingOrganisationFilter_Should_Return_Only_Matching_Organisation()
    {
        // Arrange
        var matchingId = Guid.NewGuid();

        var qualifications = new[]
        {
            new QualificationVersions
            {
                AwardingOrganisationId = matchingId
            },
            new QualificationVersions
            {
                AwardingOrganisationId = Guid.NewGuid()
            }
        }.AsQueryable();

        // Act
        var result = qualifications
            .WithAwardingOrganisationFilter([matchingId])
            .ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().AwardingOrganisationId.ShouldBe(matchingId);
    }

    [Fact]
    public void WithAllFilters_Should_Return_Only_Items_Matching_All_Criteria()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var matchingQualification = new QualificationVersions
        {
            Level = QualificationLevel.EntryLevel.ToString(),
            Type = QualificationType.AdvancedExtensionAward.ToString(),
            Ssa = SectorSubjectArea.FromFullCode("1.1").ToString(),
            AwardingOrganisationId = organisationId
        };

        var nonMatchingQualification = new QualificationVersions
        {
            Level = QualificationLevel.Level1.ToString(),
            Type = QualificationType.EssentialDigitalSkills.ToString(),
            Ssa = SectorSubjectArea.FromFullCode("2.1").ToString(),
            AwardingOrganisationId = Guid.NewGuid()
        };

        var qualifications = new[]
        {
            matchingQualification,
            nonMatchingQualification
        }.AsQueryable();

        // Act
        var result = qualifications.WithAllFilters(
                [QualificationLevel.EntryLevel.Id],
                [QualificationType.AdvancedExtensionAward.Id],
                ["1.1"],
                [organisationId])
            .ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().ShouldBe(matchingQualification);
    }
}