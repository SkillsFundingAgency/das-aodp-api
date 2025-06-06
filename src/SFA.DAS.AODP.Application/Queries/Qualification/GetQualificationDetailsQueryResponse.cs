﻿using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetQualificationDetailsQueryResponse
{
    public Guid Id { get; set; }
    public Guid QualificationId { get; set; }
    public Guid VersionFieldChangesId { get; set; }
    public string AgeGroup { get; set; }
    public string? VersionFieldChanges { get; set; }
    public string? VersionType { get; set; }
    public Guid ProcessStatusId { get; set; }
    public int AdditionalKeyChangesReceivedFlag { get; set; }
    public Guid LifecycleStageId { get; set; }
    public string? OutcomeJustificationNotes { get; set; }
    public Guid AwardingOrganisationId { get; set; }
    public string Status { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Ssa { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string SubLevel { get; set; } = null!;
    public string EqfLevel { get; set; } = null!;
    public string? GradingType { get; set; }
    public string? GradingScale { get; set; }
    public int? TotalCredits { get; set; }
    public int? Tqt { get; set; }
    public int? Glh { get; set; }
    public int? MinimumGlh { get; set; }
    public int? MaximumGlh { get; set; }
    public DateTime RegulationStartDate { get; set; }
    public DateTime OperationalStartDate { get; set; }
    public DateTime? OperationalEndDate { get; set; }
    public DateTime? CertificationEndDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public bool OfferedInEngland { get; set; }
    public bool? FundedInEngland { get; set; }
    public bool OfferedInNi { get; set; }
    public bool? OfferedInternationally { get; set; }
    public string? Specialism { get; set; }
    public string? Pathways { get; set; }
    public string? AssessmentMethods { get; set; }
    public string? ApprovedForDelFundedProgramme { get; set; }
    public string? LinkToSpecification { get; set; }
    public string? ApprenticeshipStandardReferenceNumber { get; set; }
    public string? ApprenticeshipStandardTitle { get; set; }
    public bool RegulatedByNorthernIreland { get; set; }
    public string? NiDiscountCode { get; set; }
    public string? GceSizeEquivelence { get; set; }
    public string? GcseSizeEquivelence { get; set; }
    public string? EntitlementFrameworkDesign { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public DateTime UiLastUpdatedDate { get; set; }
    public DateTime InsertedDate { get; set; }
    public DateTime? InsertedTimestamp { get; set; }
    public int? Version { get; set; }
    public bool? AppearsOnPublicRegister { get; set; }
    public int? LevelId { get; set; }
    public int? TypeId { get; set; }
    public int? SsaId { get; set; }
    public int? GradingTypeId { get; set; }
    public int? GradingScaleId { get; set; }
    public bool? PreSixteen { get; set; }
    public bool? SixteenToEighteen { get; set; }
    public bool? EighteenPlus { get; set; }
    public bool? NineteenPlus { get; set; }
    public string? ImportStatus { get; set; }
    public virtual LifecycleStage Stage { get; set; } = null!;
    public virtual AwardingOrganisation Organisation { get; set; } = null!;
    public virtual Qualification Qual { get; set; } = null!;
    public virtual ProcessStatus ProcStatus { get; set; } = null!;
    public partial class LifecycleStage
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    public partial class AwardingOrganisation
    {
        public Guid Id { get; set; }
        public int? Ukprn { get; set; }
        public string? RecognitionNumber { get; set; }
        public string? NameLegal { get; set; }
        public string? NameOfqual { get; set; }
        public string? NameGovUk { get; set; }
        public string? Name_Dsi { get; set; }
        public string? Acronym { get; set; }
    }

    public partial class Qualification
    {
        public Guid Id { get; set; }
        public string Qan { get; set; } = null!;
        public string? QualificationName { get; set; }
        public List<QualificationDiscussionHistory> QualificationDiscussionHistories { get; set; } = new List<QualificationDiscussionHistory>();

        public List<GetQualificationDetailsQueryResponse> Versions { get; set; } = new List<GetQualificationDetailsQueryResponse>();
    }

    public class PreviousVersion
    {
        public Guid Id { get; set; }
        public Guid QualificationId { get; set; }
        public Guid VersionFieldChangesId { get; set; }
        public string AgeGroup { get; set; }
        public Guid ProcessStatusId { get; set; }
        public int AdditionalKeyChangesReceivedFlag { get; set; }
        public Guid LifecycleStageId { get; set; }
        public string? OutcomeJustificationNotes { get; set; }
        public Guid AwardingOrganisationId { get; set; }
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Ssa { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string SubLevel { get; set; } = null!;
        public string EqfLevel { get; set; } = null!;
        public string? GradingType { get; set; }
        public string? GradingScale { get; set; }
        public int? TotalCredits { get; set; }
        public int? Tqt { get; set; }
        public int? Glh { get; set; }
        public int? MinimumGlh { get; set; }
        public int? MaximumGlh { get; set; }
        public DateTime RegulationStartDate { get; set; }
        public DateTime OperationalStartDate { get; set; }
        public DateTime? OperationalEndDate { get; set; }
        public DateTime? CertificationEndDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public bool OfferedInEngland { get; set; }
        public bool? FundedInEngland { get; set; }
        public bool OfferedInNi { get; set; }
        public bool? OfferedInternationally { get; set; }
        public string? Specialism { get; set; }
        public string? Pathways { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? ApprovedForDelFundedProgramme { get; set; }
        public string? LinkToSpecification { get; set; }
        public string? ApprenticeshipStandardReferenceNumber { get; set; }
        public string? ApprenticeshipStandardTitle { get; set; }
        public bool RegulatedByNorthernIreland { get; set; }
        public string? NiDiscountCode { get; set; }
        public string? GceSizeEquivelence { get; set; }
        public string? GcseSizeEquivelence { get; set; }
        public string? EntitlementFrameworkDesign { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime UiLastUpdatedDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public int? Version { get; set; }
        public bool? AppearsOnPublicRegister { get; set; }
        public int? LevelId { get; set; }
        public int? TypeId { get; set; }
        public int? SsaId { get; set; }
        public int? GradingTypeId { get; set; }
        public int? GradingScaleId { get; set; }
        public bool? PreSixteen { get; set; }
        public bool? SixteenToEighteen { get; set; }
        public bool? EighteenPlus { get; set; }
        public bool? NineteenPlus { get; set; }
        public string? ImportStatus { get; set; }

        public AwardingOrganisation Organisation { get; set; } = null!;
        public Qualification Qualification { get; set; } = null!;
    }
  
    public partial class ProcessStatus
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? IsOutcomeDecision { get; set; }
    }

    public static implicit operator GetQualificationDetailsQueryResponse(QualificationVersions entity)
    {
        return new GetQualificationDetailsQueryResponse
        {
            Id = entity.Id,
            AgeGroup = GetAgeGroup(entity),
            QualificationId = entity.QualificationId,
            VersionFieldChangesId = entity.VersionFieldChangesId,
            ProcessStatusId = entity.ProcessStatusId,
            AdditionalKeyChangesReceivedFlag = entity.AdditionalKeyChangesReceivedFlag,
            LifecycleStageId = entity.LifecycleStageId,
            OutcomeJustificationNotes = entity.OutcomeJustificationNotes,
            AwardingOrganisationId = entity.AwardingOrganisationId,
            Status = entity.Status,
            Type = entity.Type,
            Ssa = entity.Ssa,
            Level = entity.Level,
            VersionFieldChanges = entity.VersionFieldChanges.ChangedFieldNames,
            SubLevel = entity.SubLevel,
            EqfLevel = entity.EqfLevel,
            GradingType = entity.GradingType,
            GradingScale = entity.GradingScale,
            TotalCredits = entity.TotalCredits,
            Tqt = entity.Tqt,
            Glh = entity.Glh,
            MinimumGlh = entity.MinimumGlh,
            MaximumGlh = entity.MaximumGlh,
            RegulationStartDate = entity.RegulationStartDate,
            OperationalStartDate = entity.OperationalStartDate,
            OperationalEndDate = entity.OperationalEndDate,
            CertificationEndDate = entity.CertificationEndDate,
            ReviewDate = entity.ReviewDate,
            OfferedInEngland = entity.OfferedInEngland,
            FundedInEngland = entity.FundedInEngland,
            OfferedInNi = entity.OfferedInNi,
            OfferedInternationally = entity.OfferedInternationally,
            Specialism = entity.Specialism,
            Pathways = entity.Pathways,
            AssessmentMethods = entity.AssessmentMethods,
            ApprovedForDelFundedProgramme = entity.ApprovedForDelFundedProgramme,
            LinkToSpecification = entity.LinkToSpecification,
            ApprenticeshipStandardReferenceNumber = entity.ApprenticeshipStandardReferenceNumber,
            ApprenticeshipStandardTitle = entity.ApprenticeshipStandardTitle,
            RegulatedByNorthernIreland = entity.RegulatedByNorthernIreland,
            NiDiscountCode = entity.NiDiscountCode,
            GceSizeEquivelence = entity.GceSizeEquivelence,
            GcseSizeEquivelence = entity.GcseSizeEquivelence,
            EntitlementFrameworkDesign = entity.EntitlementFrameworkDesign,
            LastUpdatedDate = entity.LastUpdatedDate,
            UiLastUpdatedDate = entity.UiLastUpdatedDate,
            InsertedDate = entity.InsertedDate,
            InsertedTimestamp = entity.InsertedTimestamp,
            Version = entity.Version,
            AppearsOnPublicRegister = entity.AppearsOnPublicRegister,
            LevelId = entity.LevelId,
            TypeId = entity.TypeId,
            SsaId = entity.SsaId,
            GradingTypeId = entity.GradingTypeId,
            GradingScaleId = entity.GradingScaleId,
            PreSixteen = entity.PreSixteen,
            SixteenToEighteen = entity.SixteenToEighteen,
            EighteenPlus = entity.EighteenPlus,
            NineteenPlus = entity.NineteenPlus,
            ImportStatus = entity.ImportStatus,

            Stage = new LifecycleStage
            {
                Id = entity.LifecycleStage.Id,
                Name = entity.LifecycleStage.Name,
            },
            Organisation = new AwardingOrganisation
            {
                Id = entity.Organisation.Id,
                Ukprn = entity.Organisation.Ukprn,
                RecognitionNumber = entity.Organisation.RecognitionNumber,
                NameLegal = entity.Organisation.NameLegal,
                NameOfqual = entity.Organisation.NameOfqual,
                NameGovUk = entity.Organisation.NameGovUk,
                Name_Dsi = entity.Organisation.Name_Dsi,
                Acronym = entity.Organisation.Acronym,
            },
            Qual = new Qualification
            {
                Id = entity.Qualification.Id,
                Qan = entity.Qualification.Qan,
                QualificationName = entity.Qualification.QualificationName,
                QualificationDiscussionHistories = entity.Qualification.QualificationDiscussionHistories
                    .Select(v => new QualificationDiscussionHistory
                    {
                        Id = v.Id,
                        QualificationId = v.QualificationId,
                        ActionTypeId = v.ActionTypeId,
                        UserDisplayName = v.UserDisplayName,
                        Notes = v.Notes,
                        Timestamp = v.Timestamp,
                        ActionType = new ActionType
                        {
                            Id = v.ActionType.Id,
                            Description = v.ActionType.Description,
                        }
                    }).ToList(),
                Versions = entity.Qualification.QualificationVersions.ToList().Select(version => new GetQualificationDetailsQueryResponse()
                {
                    Id = version.Id,
                    AgeGroup = GetAgeGroup(version),
                    QualificationId = version.QualificationId,
                    VersionFieldChangesId = version.VersionFieldChangesId,
                    ProcessStatusId = version.ProcessStatusId,
                    AdditionalKeyChangesReceivedFlag = version.AdditionalKeyChangesReceivedFlag,
                    LifecycleStageId = version.LifecycleStageId,
                    OutcomeJustificationNotes = version.OutcomeJustificationNotes,
                    AwardingOrganisationId = version.AwardingOrganisationId,
                    Status = version.Status,
                    Type = version.Type,
                    Ssa = version.Ssa,
                    Level = version.Level,
                    VersionFieldChanges = version.VersionFieldChanges.ChangedFieldNames,
                    SubLevel = version.SubLevel,
                    EqfLevel = version.EqfLevel,
                    GradingType = version.GradingType,
                    GradingScale = version.GradingScale,
                    TotalCredits = version.TotalCredits,
                    Tqt = version.Tqt,
                    Glh = version.Glh,
                    MinimumGlh = version.MinimumGlh,
                    MaximumGlh = version.MaximumGlh,
                    RegulationStartDate = version.RegulationStartDate,
                    OperationalStartDate = version.OperationalStartDate,
                    OperationalEndDate = version.OperationalEndDate,
                    CertificationEndDate = version.CertificationEndDate,
                    ReviewDate = version.ReviewDate,
                    OfferedInEngland = version.OfferedInEngland,
                    OfferedInNi = version.OfferedInNi,
                    OfferedInternationally = version.OfferedInternationally,
                    Specialism = version.Specialism,
                    Pathways = version.Pathways,
                    AssessmentMethods = version.AssessmentMethods,
                    ApprovedForDelFundedProgramme = version.ApprovedForDelFundedProgramme,
                    LinkToSpecification = version.LinkToSpecification,
                    ApprenticeshipStandardReferenceNumber = version.ApprenticeshipStandardReferenceNumber,
                    ApprenticeshipStandardTitle = version.ApprenticeshipStandardTitle,
                    RegulatedByNorthernIreland = version.RegulatedByNorthernIreland,
                    NiDiscountCode = version.NiDiscountCode,
                    GceSizeEquivelence = version.GceSizeEquivelence,
                    GcseSizeEquivelence = version.GcseSizeEquivelence,
                    EntitlementFrameworkDesign = version.EntitlementFrameworkDesign,
                    LastUpdatedDate = version.LastUpdatedDate,
                    UiLastUpdatedDate = version.UiLastUpdatedDate,
                    InsertedDate = version.InsertedDate,
                    InsertedTimestamp = version.InsertedTimestamp,
                    Version = version.Version,
                    AppearsOnPublicRegister = version.AppearsOnPublicRegister,
                    LevelId = version.LevelId,
                    TypeId = version.TypeId,
                    SsaId = version.SsaId,
                    GradingTypeId = version.GradingTypeId,
                    GradingScaleId = version.GradingScaleId,
                    PreSixteen = version.PreSixteen,
                    SixteenToEighteen = version.SixteenToEighteen,
                    EighteenPlus = version.EighteenPlus,
                    NineteenPlus = version.NineteenPlus,
                    ImportStatus = version.ImportStatus,

                    Stage = new LifecycleStage
                    {
                        Id = version.LifecycleStage.Id,
                        Name = version.LifecycleStage.Name,
                    },
                    Organisation = new AwardingOrganisation
                    {
                        Id = version.Organisation.Id,
                        Ukprn = version.Organisation.Ukprn,
                        RecognitionNumber = version.Organisation.RecognitionNumber,
                        NameLegal = version.Organisation.NameLegal,
                        NameOfqual = version.Organisation.NameOfqual,
                        NameGovUk = version.Organisation.NameGovUk,
                        Name_Dsi = version.Organisation.Name_Dsi,
                        Acronym = version.Organisation.Acronym,
                    },
                    Qual = new Qualification
                    {
                        Id = version.Qualification.Id,
                        Qan = version.Qualification.Qan,
                        QualificationName = version.Qualification.QualificationName,
                    }
                }).ToList()
            },
            ProcStatus = new ProcessStatus()
            {
                Id = entity.ProcessStatus.Id,
                Name = entity.ProcessStatus.Name,
                IsOutcomeDecision = entity.ProcessStatus.IsOutcomeDecision,
            }
        };
    }

    private static string GetAgeGroup(QualificationVersions entity)
    {
        if (entity.PreSixteen == true)
        {
            return "<16";

        }
        else if (entity.SixteenToEighteen == true)
        {
            return "16 - 18";
        }
        else if (entity.EighteenPlus == true)
        {
            return "18+";
        }
        else if (entity.NineteenPlus == true)
        {
            return "19+";
        }
        else return "";
    }
}