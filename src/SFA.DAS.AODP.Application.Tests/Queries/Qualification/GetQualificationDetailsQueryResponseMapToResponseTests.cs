using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using Shouldly;
using QualificationEntity = SFA.DAS.AODP.Data.Entities.Qualification.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification;

public class GetQualificationDetailsQueryResponseMapToResponseTests
{
    [Fact]
    public void MapToResponse_WithValidEntity_MapsAllSimpleProperties()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert - Simple scalar properties
        result.Id.ShouldBe(entity.Id);
        result.Name.ShouldBe(entity.Name);
        result.QualificationId.ShouldBe(entity.QualificationId);
        result.VersionFieldChangesId.ShouldBe(entity.VersionFieldChangesId);
        result.ProcessStatusId.ShouldBe(entity.ProcessStatusId);
        result.AdditionalKeyChangesReceivedFlag.ShouldBe(entity.AdditionalKeyChangesReceivedFlag);
        result.LifecycleStageId.ShouldBe(entity.LifecycleStageId);
        result.OutcomeJustificationNotes.ShouldBe(entity.OutcomeJustificationNotes);
        result.AwardingOrganisationId.ShouldBe(entity.AwardingOrganisationId);
        result.Status.ShouldBe(entity.Status);
        result.Type.ShouldBe(entity.Type);
        result.Ssa.ShouldBe(entity.Ssa);
        result.Level.ShouldBe(entity.Level);
        result.SubLevel.ShouldBe(entity.SubLevel);
        result.EqfLevel.ShouldBe(entity.EqfLevel);
        result.GradingType.ShouldBe(entity.GradingType);
        result.GradingScale.ShouldBe(entity.GradingScale);
        result.TotalCredits.ShouldBe(entity.TotalCredits);
        result.Tqt.ShouldBe(entity.Tqt);
        result.Glh.ShouldBe(entity.Glh);
        result.MinimumGlh.ShouldBe(entity.MinimumGlh);
        result.MaximumGlh.ShouldBe(entity.MaximumGlh);
        result.RegulationStartDate.ShouldBe(entity.RegulationStartDate);
        result.OperationalStartDate.ShouldBe(entity.OperationalStartDate);
        result.OperationalEndDate.ShouldBe(entity.OperationalEndDate);
        result.CertificationEndDate.ShouldBe(entity.CertificationEndDate);
        result.ReviewDate.ShouldBe(entity.ReviewDate);
        result.OfferedInEngland.ShouldBe(entity.OfferedInEngland);
        result.OfferedInNi.ShouldBe(entity.OfferedInNi);
        result.OfferedInternationally.ShouldBe(entity.OfferedInternationally);
        result.Specialism.ShouldBe(entity.Specialism);
        result.Pathways.ShouldBe(entity.Pathways);
        result.AssessmentMethods.ShouldBe(entity.AssessmentMethods);
        result.ApprovedForDelFundedProgramme.ShouldBe(entity.ApprovedForDelFundedProgramme);
        result.LinkToSpecification.ShouldBe(entity.LinkToSpecification);
        result.ApprenticeshipStandardReferenceNumber.ShouldBe(entity.ApprenticeshipStandardReferenceNumber);
        result.ApprenticeshipStandardTitle.ShouldBe(entity.ApprenticeshipStandardTitle);
        result.RegulatedByNorthernIreland.ShouldBe(entity.RegulatedByNorthernIreland);
        result.NiDiscountCode.ShouldBe(entity.NiDiscountCode);
        result.GceSizeEquivelence.ShouldBe(entity.GceSizeEquivelence);
        result.GcseSizeEquivelence.ShouldBe(entity.GcseSizeEquivelence);
        result.EntitlementFrameworkDesign.ShouldBe(entity.EntitlementFrameworkDesign);
        result.LastUpdatedDate.ShouldBe(entity.LastUpdatedDate);
        result.UiLastUpdatedDate.ShouldBe(entity.UiLastUpdatedDate);
        result.InsertedDate.ShouldBe(entity.InsertedDate);
        result.InsertedTimestamp.ShouldBe(entity.InsertedTimestamp);
        result.Version.ShouldBe(entity.Version);
        result.AppearsOnPublicRegister.ShouldBe(entity.AppearsOnPublicRegister);
        result.LevelId.ShouldBe(entity.LevelId);
        result.TypeId.ShouldBe(entity.TypeId);
        result.SsaId.ShouldBe(entity.SsaId);
        result.GradingTypeId.ShouldBe(entity.GradingTypeId);
        result.GradingScaleId.ShouldBe(entity.GradingScaleId);
        result.PreSixteen.ShouldBe(entity.PreSixteen);
        result.SixteenToEighteen.ShouldBe(entity.SixteenToEighteen);
        result.EighteenPlus.ShouldBe(entity.EighteenPlus);
        result.NineteenPlus.ShouldBe(entity.NineteenPlus);
        result.ImportStatus.ShouldBe(entity.ImportStatus);
    }

    [Fact]
    public void MapToResponse_WithValidEntity_MapsVersionFieldChangesFromEntity()
    {
        // Arrange
        var changedFieldNames = "Field1,Field2,Field3";
        var entity = CreateQualificationVersionEntity();
        entity.VersionFieldChanges = new VersionFieldChange { ChangedFieldNames = changedFieldNames };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.VersionFieldChanges.ShouldBe(changedFieldNames);
    }

    [Fact]
    public void MapToResponse_WithValidEntity_MapsLifecycleStage()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var lifecycleStageId = Guid.NewGuid();
        var lifecycleStageName = "Active Stage";
        entity.LifecycleStage = new LifecycleStage { Id = lifecycleStageId, Name = lifecycleStageName };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Stage.ShouldNotBeNull();
        result.Stage.Id.ShouldBe(lifecycleStageId);
        result.Stage.Name.ShouldBe(lifecycleStageName);
    }

    [Fact]
    public void MapToResponse_WithValidEntity_MapsAwardingOrganisation()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var orgId = Guid.NewGuid();
        var ukprn = 12345678;
        var recognitionNumber = "REC123";
        var nameLegal = "Legal Name Ltd";
        var nameOfqual = "Ofqual Name";
        var nameGovUk = "Gov.uk Name";
        var nameDsi = "DSI Name";
        var acronym = "ACRO";

        entity.Organisation = new AwardingOrganisation
        {
            Id = orgId,
            Ukprn = ukprn,
            RecognitionNumber = recognitionNumber,
            NameLegal = nameLegal,
            NameOfqual = nameOfqual,
            NameGovUk = nameGovUk,
            Name_Dsi = nameDsi,
            Acronym = acronym
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Organisation.ShouldNotBeNull();
        result.Organisation.Id.ShouldBe(orgId);
        result.Organisation.Ukprn.ShouldBe(ukprn);
        result.Organisation.RecognitionNumber.ShouldBe(recognitionNumber);
        result.Organisation.NameLegal.ShouldBe(nameLegal);
        result.Organisation.NameOfqual.ShouldBe(nameOfqual);
        result.Organisation.NameGovUk.ShouldBe(nameGovUk);
        result.Organisation.Name_Dsi.ShouldBe(nameDsi);
        result.Organisation.Acronym.ShouldBe(acronym);
    }

    [Fact]
    public void MapToResponse_WithValidEntity_MapsQualification()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var qualId = Guid.NewGuid();
        var qan = "QAN123456";
        var qualificationName = "Test Qualification Name";

        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = qan,
            QualificationName = qualificationName,
            QualificationDiscussionHistories = new List<QualificationDiscussionHistory>()
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.ShouldNotBeNull();
        result.Qual.Id.ShouldBe(qualId);
        result.Qual.Qan.ShouldBe(qan);
        result.Qual.QualificationName.ShouldBe(qualificationName);
    }

    [Fact]
    public void MapToResponse_WithQualificationDiscussionHistories_MapsThem()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var discussionHistoryId = Guid.NewGuid();
        var qualId = Guid.NewGuid();
        var actionTypeId = Guid.NewGuid();
        var userDisplayName = "John Doe";
        var notes = "Test notes";
        var timestamp = DateTime.UtcNow;

        var actionType = new ActionType
        {
            Id = actionTypeId,
            Description = "Approved"
        };

        var discussionHistory = new QualificationDiscussionHistory
        {
            Id = discussionHistoryId,
            QualificationId = qualId,
            ActionTypeId = actionTypeId,
            UserDisplayName = userDisplayName,
            Notes = notes,
            Timestamp = timestamp,
            ActionType = actionType
        };

        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = "QAN123",
            QualificationName = "Test Qual",
            QualificationDiscussionHistories = new List<QualificationDiscussionHistory> { discussionHistory }
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.QualificationDiscussionHistories.ShouldNotBeNull();
        result.Qual.QualificationDiscussionHistories.ShouldHaveSingleItem();
        var mappedHistory = result.Qual.QualificationDiscussionHistories.First();
        mappedHistory.Id.ShouldBe(discussionHistoryId);
        mappedHistory.QualificationId.ShouldBe(qualId);
        mappedHistory.ActionTypeId.ShouldBe(actionTypeId);
        mappedHistory.UserDisplayName.ShouldBe(userDisplayName);
        mappedHistory.Notes.ShouldBe(notes);
        mappedHistory.Timestamp.ShouldBe(timestamp);
        mappedHistory.ActionType.ShouldNotBeNull();
        mappedHistory.ActionType.Id.ShouldBe(actionTypeId);
        mappedHistory.ActionType.Description.ShouldBe("Approved");
    }

    [Fact]
    public void MapToResponse_WithMultipleQualificationDiscussionHistories_MapsAll()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var qualId = Guid.NewGuid();

        var historyList = new List<QualificationDiscussionHistory>();
        for (int i = 0; i < 3; i++)
        {
            historyList.Add(new QualificationDiscussionHistory
            {
                Id = Guid.NewGuid(),
                QualificationId = qualId,
                ActionTypeId = Guid.NewGuid(),
                UserDisplayName = $"User {i}",
                Notes = $"Notes {i}",
                Timestamp = DateTime.UtcNow.AddDays(-i),
                ActionType = new ActionType { Id = Guid.NewGuid(), Description = $"Action {i}" }
            });
        }

        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = "QAN123",
            QualificationName = "Test Qual",
            QualificationDiscussionHistories = historyList
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.QualificationDiscussionHistories.Count.ShouldBe(3);
        for (int i = 0; i < 3; i++)
        {
            result.Qual.QualificationDiscussionHistories[i].UserDisplayName.ShouldBe($"User {i}");
            result.Qual.QualificationDiscussionHistories[i].Notes.ShouldBe($"Notes {i}");
        }
    }

    [Fact]
    public void MapToResponse_WithQualificationVersions_MapsVersionsList()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var qualId = Guid.NewGuid();

        var version1 = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            Name = "Version 1",
            QualificationId = qualId,
            VersionFieldChangesId = Guid.NewGuid(),
            ProcessStatusId = Guid.NewGuid(),
            AdditionalKeyChangesReceivedFlag = 0,
            LifecycleStageId = Guid.NewGuid(),
            AwardingOrganisationId = Guid.NewGuid(),
            Status = "Active",
            Type = "Type1",
            Ssa = "SSA1",
            Level = "Level 3",
            SubLevel = "SubLevel",
            EqfLevel = "5",
            PreSixteen = false,
            SixteenToEighteen = true,
            EighteenPlus = false,
            NineteenPlus = false,
            RegulationStartDate = DateTime.UtcNow,
            OperationalStartDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            UiLastUpdatedDate = DateTime.UtcNow,
            InsertedDate = DateTime.UtcNow,
            OfferedInEngland = true,
            OfferedInNi = false,
            RegulatedByNorthernIreland = false,
            VersionFieldChanges = new VersionFieldChange { ChangedFieldNames = "Field1" },
            LifecycleStage = new LifecycleStage { Id = Guid.NewGuid(), Name = "Active" },
            Organisation = new AwardingOrganisation { Id = Guid.NewGuid() },
            ProcessStatus = new ProcessStatus { Id = Guid.NewGuid(), Name = "Approved" },
            Qualification = new QualificationEntity
            {
                Id = qualId,
                Qan = "QAN001",
                QualificationName = "Qual 1",
                QualificationVersions = new List<QualificationVersions>()
            }
        };

        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = "QAN123",
            QualificationName = "Test Qual",
            QualificationVersions = new List<QualificationVersions> { version1 },
            QualificationDiscussionHistories = new List<QualificationDiscussionHistory>()
        };

        entity.VersionFieldChanges = new VersionFieldChange { ChangedFieldNames = "Field1" };
        entity.LifecycleStage = new LifecycleStage { Id = Guid.NewGuid(), Name = "Active" };
        entity.Organisation = new AwardingOrganisation { Id = Guid.NewGuid() };
        entity.ProcessStatus = new ProcessStatus { Id = Guid.NewGuid(), Name = "Approved" };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.Versions.ShouldNotBeNull();
        result.Qual.Versions.ShouldHaveSingleItem();
        var mappedVersion = result.Qual.Versions.First();
        mappedVersion.Id.ShouldBe(version1.Id);
        mappedVersion.Name.ShouldBe(version1.Name);
        mappedVersion.QualificationId.ShouldBe(version1.QualificationId);
        mappedVersion.AgeGroup.ShouldBe("16 - 18");
    }

    [Fact]
    public void MapToResponse_WithProcessStatus_MapsProcStatus()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var procStatusId = Guid.NewGuid();
        var procStatusName = "Approved";
        var isOutcomeDecision = 1;

        entity.ProcessStatus = new ProcessStatus
        {
            Id = procStatusId,
            Name = procStatusName,
            IsOutcomeDecision = isOutcomeDecision
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.ProcStatus.ShouldNotBeNull();
        result.ProcStatus.Id.ShouldBe(procStatusId);
        result.ProcStatus.Name.ShouldBe(procStatusName);
        result.ProcStatus.IsOutcomeDecision.ShouldBe(isOutcomeDecision);
    }

    [Theory]
    [InlineData(true, false, false, false, "<16")]
    [InlineData(false, true, false, false, "16 - 18")]
    [InlineData(false, false, true, false, "18+")]
    [InlineData(false, false, false, true, "19+")]
    [InlineData(false, false, false, false, "")]
    [InlineData(null, null, null, null, "")]
    public void MapToResponse_WithDifferentAgeGroupCombinations_MapsCorrectAgeGroup(
        bool? preSixteen, bool? sixteenToEighteen, bool? eighteenPlus, bool? nineteenPlus, string expectedAgeGroup)
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.PreSixteen = preSixteen;
        entity.SixteenToEighteen = sixteenToEighteen;
        entity.EighteenPlus = eighteenPlus;
        entity.NineteenPlus = nineteenPlus;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.AgeGroup.ShouldBe(expectedAgeGroup);
    }

    [Fact]
    public void MapToResponse_WithNullableIntProperties_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.TotalCredits = null;
        entity.Tqt = null;
        entity.Glh = null;
        entity.MinimumGlh = null;
        entity.MaximumGlh = null;
        entity.LevelId = null;
        entity.TypeId = null;
        entity.SsaId = null;
        entity.GradingTypeId = null;
        entity.GradingScaleId = null;
        entity.Version = null;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.TotalCredits.ShouldBeNull();
        result.Tqt.ShouldBeNull();
        result.Glh.ShouldBeNull();
        result.MinimumGlh.ShouldBeNull();
        result.MaximumGlh.ShouldBeNull();
        result.LevelId.ShouldBeNull();
        result.TypeId.ShouldBeNull();
        result.SsaId.ShouldBeNull();
        result.GradingTypeId.ShouldBeNull();
        result.GradingScaleId.ShouldBeNull();
        result.Version.ShouldBeNull();
    }

    [Fact]
    public void MapToResponse_WithNullableBoolProperties_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.OfferedInternationally = null;
        entity.AppearsOnPublicRegister = null;
        entity.PreSixteen = null;
        entity.SixteenToEighteen = null;
        entity.EighteenPlus = null;
        entity.NineteenPlus = null;
        entity.IntentionToSeekFundingInEngland = null;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.OfferedInternationally.ShouldBeNull();
        result.AppearsOnPublicRegister.ShouldBeNull();
        result.PreSixteen.ShouldBeNull();
        result.SixteenToEighteen.ShouldBeNull();
        result.EighteenPlus.ShouldBeNull();
        result.NineteenPlus.ShouldBeNull();
        result.IntentionToSeekFundingInEngland.ShouldBeNull();
    }

    [Fact]
    public void MapToResponse_WithNullableDateTimeProperties_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.OperationalEndDate = null;
        entity.CertificationEndDate = null;
        entity.ReviewDate = null;
        entity.InsertedTimestamp = null;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.OperationalEndDate.ShouldBeNull();
        result.CertificationEndDate.ShouldBeNull();
        result.ReviewDate.ShouldBeNull();
        result.InsertedTimestamp.ShouldBeNull();
    }

    [Fact]
    public void MapToResponse_WithNullableStringProperties_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.GradingType = null;
        entity.GradingScale = null;
        entity.OutcomeJustificationNotes = null;
        entity.Specialism = null;
        entity.Pathways = null;
        entity.AssessmentMethods = null;
        entity.ApprovedForDelFundedProgramme = null;
        entity.LinkToSpecification = null;
        entity.ApprenticeshipStandardReferenceNumber = null;
        entity.ApprenticeshipStandardTitle = null;
        entity.NiDiscountCode = null;
        entity.GceSizeEquivelence = null;
        entity.GcseSizeEquivelence = null;
        entity.EntitlementFrameworkDesign = null;
        entity.ImportStatus = null;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.GradingType.ShouldBeNull();
        result.GradingScale.ShouldBeNull();
        result.OutcomeJustificationNotes.ShouldBeNull();
        result.Specialism.ShouldBeNull();
        result.Pathways.ShouldBeNull();
        result.AssessmentMethods.ShouldBeNull();
        result.ApprovedForDelFundedProgramme.ShouldBeNull();
        result.LinkToSpecification.ShouldBeNull();
        result.ApprenticeshipStandardReferenceNumber.ShouldBeNull();
        result.ApprenticeshipStandardTitle.ShouldBeNull();
        result.NiDiscountCode.ShouldBeNull();
        result.GceSizeEquivelence.ShouldBeNull();
        result.GcseSizeEquivelence.ShouldBeNull();
        result.EntitlementFrameworkDesign.ShouldBeNull();
        result.ImportStatus.ShouldBeNull();
    }

    [Fact]
    public void MapToResponse_WithEmptyQualificationDiscussionHistories_MapsEmptyList()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var qualId = Guid.NewGuid();
        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = "QAN123",
            QualificationName = "Test Qual",
            QualificationDiscussionHistories = new List<QualificationDiscussionHistory>()
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.QualificationDiscussionHistories.ShouldNotBeNull();
        result.Qual.QualificationDiscussionHistories.ShouldBeEmpty();
    }

    [Fact]
    public void MapToResponse_WithEmptyQualificationVersions_MapsEmptyList()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        var qualId = Guid.NewGuid();
        entity.Qualification = new QualificationEntity
        {
            Id = qualId,
            Qan = "QAN123",
            QualificationName = "Test Qual",
            QualificationVersions = new List<QualificationVersions>(),
            QualificationDiscussionHistories = new List<QualificationDiscussionHistory>()
        };

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.Qual.Versions.ShouldNotBeNull();
        result.Qual.Versions.ShouldBeEmpty();
    }

    [Fact]
    public void MapToResponse_WithAllDateTimeValues_MapsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = CreateQualificationVersionEntity();
        entity.RegulationStartDate = now.AddDays(-365);
        entity.OperationalStartDate = now.AddDays(-300);
        entity.OperationalEndDate = now.AddDays(100);
        entity.CertificationEndDate = now.AddDays(200);
        entity.ReviewDate = now.AddDays(50);
        entity.LastUpdatedDate = now.AddHours(-24);
        entity.UiLastUpdatedDate = now.AddHours(-12);
        entity.InsertedDate = now.AddDays(-365);
        entity.InsertedTimestamp = now.AddDays(-365);

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.RegulationStartDate.ShouldBe(entity.RegulationStartDate);
        result.OperationalStartDate.ShouldBe(entity.OperationalStartDate);
        result.OperationalEndDate.ShouldBe(entity.OperationalEndDate);
        result.CertificationEndDate.ShouldBe(entity.CertificationEndDate);
        result.ReviewDate.ShouldBe(entity.ReviewDate);
        result.LastUpdatedDate.ShouldBe(entity.LastUpdatedDate);
        result.UiLastUpdatedDate.ShouldBe(entity.UiLastUpdatedDate);
        result.InsertedDate.ShouldBe(entity.InsertedDate);
        result.InsertedTimestamp.ShouldBe(entity.InsertedTimestamp);
    }

    [Fact]
    public void MapToResponse_WithAllBooleanValues_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.OfferedInEngland = true;
        entity.OfferedInNi = false;
        entity.OfferedInternationally = true;
        entity.RegulatedByNorthernIreland = false;
        entity.AppearsOnPublicRegister = true;
        entity.PreSixteen = false;
        entity.SixteenToEighteen = true;
        entity.EighteenPlus = false;
        entity.NineteenPlus = true;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.OfferedInEngland.ShouldBeTrue();
        result.OfferedInNi.ShouldBeFalse();
        result.OfferedInternationally.ShouldBe(true);
        result.RegulatedByNorthernIreland.ShouldBeFalse();
        result.AppearsOnPublicRegister.ShouldBe(true);
        result.PreSixteen.ShouldBe(false);
        result.SixteenToEighteen.ShouldBe(true);
        result.EighteenPlus.ShouldBe(false);
        result.NineteenPlus.ShouldBe(true);
    }

    [Fact]
    public void MapToResponse_WithAllIntegerValues_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.AdditionalKeyChangesReceivedFlag = 42;
        entity.TotalCredits = 100;
        entity.Tqt = 200;
        entity.Glh = 150;
        entity.MinimumGlh = 100;
        entity.MaximumGlh = 200;
        entity.LevelId = 3;
        entity.TypeId = 5;
        entity.SsaId = 7;
        entity.GradingTypeId = 9;
        entity.GradingScaleId = 11;
        entity.Version = 2;

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        result.AdditionalKeyChangesReceivedFlag.ShouldBe(42);
        result.TotalCredits.ShouldBe(100);
        result.Tqt.ShouldBe(200);
        result.Glh.ShouldBe(150);
        result.MinimumGlh.ShouldBe(100);
        result.MaximumGlh.ShouldBe(200);
        result.LevelId.ShouldBe(3);
        result.TypeId.ShouldBe(5);
        result.SsaId.ShouldBe(7);
        result.GradingTypeId.ShouldBe(9);
        result.GradingScaleId.ShouldBe(11);
        result.Version.ShouldBe(2);
    }

    [Fact]
    public void MapToResponse_WithAllStringValues_MapsCorrectly()
    {
        // Arrange
        var entity = CreateQualificationVersionEntity();
        entity.Name = "Detailed Qualification Name";
        entity.Status = "StatusValue";
        entity.Type = "TypeValue";
        entity.Ssa = "SSAValue";
        entity.Level = "LevelValue";
        entity.SubLevel = "SubLevelValue";
        entity.EqfLevel = "EqfLevelValue";
        entity.GradingType = "GradingTypeValue";
        entity.GradingScale = "GradingScaleValue";
        entity.OutcomeJustificationNotes = "OutcomeNotes";
        entity.Specialism = "SpecialismValue";
        entity.Pathways = "PathwaysValue";
        entity.AssessmentMethods = "AssessmentValue";
        entity.ApprovedForDelFundedProgramme = "ApprovedValue";
        entity.LinkToSpecification = "LinkValue";
        entity.ApprenticeshipStandardReferenceNumber = "RefNumber";
        entity.ApprenticeshipStandardTitle = "RefTitle";
        entity.NiDiscountCode = "DiscountCode";
        entity.GceSizeEquivelence = "GceEquiv";
        entity.GcseSizeEquivelence = "GcseEquiv";
        entity.EntitlementFrameworkDesign = "FrameworkDesign";
        entity.ImportStatus = "ImportStatusValue";

        // Act
        var result = GetQualificationDetailsQueryResponse.MapToResponse(entity);

        // Assert
        Assert.Equal("Detailed Qualification Name", result.Name);
        Assert.Equal("StatusValue", result.Status);
        Assert.Equal("TypeValue", result.Type);
        Assert.Equal("SSAValue", result.Ssa);
        Assert.Equal("LevelValue", result.Level);
        Assert.Equal("SubLevelValue", result.SubLevel);
        Assert.Equal("EqfLevelValue", result.EqfLevel);
        Assert.Equal("GradingTypeValue", result.GradingType);
        Assert.Equal("GradingScaleValue", result.GradingScale);
        Assert.Equal("OutcomeNotes", result.OutcomeJustificationNotes);
        Assert.Equal("SpecialismValue", result.Specialism);
        Assert.Equal("PathwaysValue", result.Pathways);
        Assert.Equal("AssessmentValue", result.AssessmentMethods);
        Assert.Equal("ApprovedValue", result.ApprovedForDelFundedProgramme);
        Assert.Equal("LinkValue", result.LinkToSpecification);
        Assert.Equal("RefNumber", result.ApprenticeshipStandardReferenceNumber);
        Assert.Equal("RefTitle", result.ApprenticeshipStandardTitle);
        Assert.Equal("DiscountCode", result.NiDiscountCode);
        Assert.Equal("GceEquiv", result.GceSizeEquivelence);
        Assert.Equal("GcseEquiv", result.GcseSizeEquivelence);
        Assert.Equal("FrameworkDesign", result.EntitlementFrameworkDesign);
        Assert.Equal("ImportStatusValue", result.ImportStatus);
    }

    // Helper method to create a valid QualificationVersions entity
    private static QualificationVersions CreateQualificationVersionEntity()
    {
        var qualId = Guid.NewGuid();
        return new QualificationVersions
        {
            Id = Guid.NewGuid(),
            Name = "Test Qualification",
            QualificationId = qualId,
            VersionFieldChangesId = Guid.NewGuid(),
            ProcessStatusId = Guid.NewGuid(),
            AdditionalKeyChangesReceivedFlag = 0,
            LifecycleStageId = Guid.NewGuid(),
            AwardingOrganisationId = Guid.NewGuid(),
            Status = "Active",
            Type = "Award",
            Ssa = "13",
            Level = "3",
            SubLevel = "SL",
            EqfLevel = "5",
            GradingType = "Pass/Fail",
            GradingScale = "1-100",
            TotalCredits = 60,
            Tqt = 120,
            Glh = 90,
            MinimumGlh = 80,
            MaximumGlh = 100,
            RegulationStartDate = DateTime.UtcNow.AddDays(-365),
            OperationalStartDate = DateTime.UtcNow.AddDays(-300),
            OperationalEndDate = DateTime.UtcNow.AddDays(100),
            CertificationEndDate = DateTime.UtcNow.AddDays(200),
            ReviewDate = DateTime.UtcNow.AddDays(50),
            OfferedInEngland = true,
            OfferedInNi = false,
            OfferedInternationally = false,
            Specialism = "Specialism1",
            Pathways = "Pathway1,Pathway2",
            AssessmentMethods = "Method1,Method2",
            ApprovedForDelFundedProgramme = "Yes",
            LinkToSpecification = "http://example.com",
            ApprenticeshipStandardReferenceNumber = "ASRN001",
            ApprenticeshipStandardTitle = "Apprenticeship Title",
            RegulatedByNorthernIreland = false,
            NiDiscountCode = "NDC001",
            GceSizeEquivelence = "GceEquiv",
            GcseSizeEquivelence = "GcseEquiv",
            EntitlementFrameworkDesign = "FrameworkDesign",
            LastUpdatedDate = DateTime.UtcNow.AddHours(-24),
            UiLastUpdatedDate = DateTime.UtcNow.AddHours(-12),
            InsertedDate = DateTime.UtcNow.AddDays(-365),
            InsertedTimestamp = DateTime.UtcNow.AddDays(-365),
            Version = 1,
            AppearsOnPublicRegister = true,
            LevelId = 3,
            TypeId = 1,
            SsaId = 13,
            GradingTypeId = 1,
            GradingScaleId = 1,
            PreSixteen = false,
            SixteenToEighteen = true,
            EighteenPlus = false,
            NineteenPlus = false,
            ImportStatus = "Imported",
            IntentionToSeekFundingInEngland = true,
            VersionFieldChanges = new VersionFieldChange { ChangedFieldNames = "Field1,Field2" },
            LifecycleStage = new LifecycleStage { Id = Guid.NewGuid(), Name = "Active" },
            Organisation = new AwardingOrganisation
            {
                Id = Guid.NewGuid(),
                Ukprn = 12345678,
                RecognitionNumber = "REC001"
            },
            ProcessStatus = new ProcessStatus
            {
                Id = Guid.NewGuid(),
                Name = "Approved",
                IsOutcomeDecision = 0
            },
            Qualification = new QualificationEntity
            {
                Id = qualId,
                Qan = "QAN123456",
                QualificationName = "Test Qualification",
                QualificationDiscussionHistories = new List<QualificationDiscussionHistory>(),
                QualificationVersions = new List<QualificationVersions>()
            }
        };
    }
}
