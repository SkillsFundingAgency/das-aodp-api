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
        var matchingQualification = new QualificationVersions { Level = targetLevel };
        var nonMatchingQualification = new QualificationVersions { Level = QualificationLevel.Level2.ToString() };

        var rolloverCandidates = new[]
        {
            new RolloverCandidates
            {
                QualificationVersion = matchingQualification
            },
            new RolloverCandidates
            {
                QualificationVersion = nonMatchingQualification
            }
        }.AsQueryable();

        var levelIds = new[]
        {
            QualificationLevel.Level1.Id
        };

        // Act
        var result = rolloverCandidates.WithLevelFilter(levelIds).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationVersion.Level.ShouldBe(targetLevel);
    }

    [Fact]
    public void WithTypeFilter_Should_Return_Only_Matching_Types()
    {
        // Arrange
        var targetType = QualificationType.AccessToHigherEducation.ToString();
        var matchingQualification = new QualificationVersions { Type = targetType };
        var nonMatchingQualification = new QualificationVersions { Type = QualificationType.AdvancedExtensionAward.ToString()};

        var rolloverCandidates = new[]
        {
            new RolloverCandidates
            {
                QualificationVersion = matchingQualification
            },
            new RolloverCandidates
            {
                QualificationVersion = nonMatchingQualification
            }
        }.AsQueryable();

        var typeIds = new[]
        {
            QualificationType.AccessToHigherEducation.Id
        };

        // Act
        var result = rolloverCandidates.WithTypeFilter(typeIds).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationVersion.Type.ShouldBe(targetType);
    }

    [Fact]
    public void WithSectorSubjectAreaFilter_Should_Return_Only_Matching_Ssa()
    {
        // Arrange
        var targetSsa = SectorSubjectArea.FromFullCode("1.1").ToString();
        var matchingQualification = new QualificationVersions { Ssa = targetSsa };
        var nonMatchingQualification = new QualificationVersions { Ssa = SectorSubjectArea.FromFullCode("2.1").ToString() };

        var rolloverCandidates = new[]
        {
            new RolloverCandidates
            {
                QualificationVersion = matchingQualification
            },
            new RolloverCandidates
            {
                QualificationVersion = nonMatchingQualification
            }
        }.AsQueryable();

        var ssaCodes = new[] { "1.1" };

        // Act
        var result = rolloverCandidates.WithSectorSubjectAreaFilter(ssaCodes).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationVersion.Ssa.ShouldBe(targetSsa);
    }

    [Fact]
    public void WithAwardingOrganisationFilter_Should_Return_Only_Matching_Organisation()
    {
        // Arrange
        var matchingId = Guid.NewGuid();
        var matchingQualification = new QualificationVersions {
            AwardingOrganisationId = matchingId,
            Organisation = new AwardingOrganisation
            {
                RecognitionNumber = "RN12345"
            }
        };
        var nonMatchingQualification = new QualificationVersions
        {
            AwardingOrganisationId = Guid.NewGuid(),
            Organisation = new AwardingOrganisation
            {
                RecognitionNumber = "RN4567"
            }
        };

        var rolloverCandidates = new[]
        {
            new RolloverCandidates
            {
                QualificationVersion = matchingQualification
            },
            new RolloverCandidates
            {
                QualificationVersion = nonMatchingQualification
            }
        }.AsQueryable();

        // Act
        var result = rolloverCandidates
            .WithAwardingOrganisationFilter(["RN12345"])
            .ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationVersion.AwardingOrganisationId.ShouldBe(matchingId);
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
            AwardingOrganisationId = organisationId,
            Organisation = new AwardingOrganisation
            {
                RecognitionNumber = "RN12345"
            }
        };

        var nonMatchingQualification = new QualificationVersions
        {
            Level = QualificationLevel.Level1.ToString(),
            Type = QualificationType.EssentialDigitalSkills.ToString(),
            Ssa = SectorSubjectArea.FromFullCode("2.1").ToString(),
            AwardingOrganisationId = Guid.NewGuid(),
            Organisation = new AwardingOrganisation
            {
                RecognitionNumber = "RN5674"
            }
        };

        var rolloverCandidates = new[]
        {
            new RolloverCandidates
            {
                QualificationVersion = matchingQualification
            },
            new RolloverCandidates
            {
                QualificationVersion = nonMatchingQualification
            }
        }.AsQueryable();

        // Act
        var result = rolloverCandidates.WithAllFilters(
                [QualificationLevel.EntryLevel.Id],
                [QualificationType.AdvancedExtensionAward.Id],
                ["1.1"],
                ["RN12345"])
            .ToList();

        // Assert
        result.Count.ShouldBe(1);
        result.Single().QualificationVersion.ShouldBe(matchingQualification);
    }
}