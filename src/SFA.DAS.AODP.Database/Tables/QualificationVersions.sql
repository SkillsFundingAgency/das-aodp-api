CREATE TABLE [regulated].[QualificationVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[VersionFieldChangesId] [uniqueidentifier] NOT NULL,
	[ProcessStatusId] [uniqueidentifier] NOT NULL,
	[AdditionalKeyChangesReceivedFlag] [int] NOT NULL,
	[LifecycleStageId] [uniqueidentifier] NOT NULL,
	[OutcomeJustificationNotes] [varchar](max) NULL,
	[AwardingOrganisationId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](100) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Ssa] [varchar](150) NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[SubLevel] [varchar](50) NOT NULL,
	[EqfLevel] [varchar](50) NOT NULL,
	[GradingType] [varchar](50) NULL,
	[GradingScale] [varchar](250) NULL,
	[TotalCredits] [int] NULL,
	[Tqt] [int] NULL,
	[Glh] [int] NULL,
	[MinimumGlh] [int] NULL,
	[MaximumGlh] [int] NULL,
	[RegulationStartDate] [datetime] NOT NULL,
	[OperationalStartDate] [datetime] NOT NULL,
	[OperationalEndDate] [datetime] NULL,
	[CertificationEndDate] [datetime] NULL,
	[ReviewDate] [datetime] NULL,
	[OfferedInEngland] [bit] NOT NULL,
	[OfferedInNi] [bit] NOT NULL,
	[OfferedInternationally] [bit] NULL,
	[Specialism] [varchar](max) NULL,
	[Pathways] [varchar](max) NULL,
	[AssessmentMethods] [varchar](max) NULL,
	[ApprovedForDelFundedProgramme] [varchar](150) NULL,
	[LinkToSpecification] [varchar](max) NULL,
	[ApprenticeshipStandardReferenceNumber] [varchar](50) NULL,
	[ApprenticeshipStandardTitle] [varchar](150) NULL,
	[RegulatedByNorthernIreland] [bit] NOT NULL,
	[NiDiscountCode] [varchar](150) NULL,
	[GceSizeEquivelence] [varchar](50) NULL,
	[GcseSizeEquivelence] [varchar](50) NULL,
	[EntitlementFrameworkDesign] [varchar](50) NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
	[UiLastUpdatedDate] [datetime] NOT NULL,
	[InsertedDate] [datetime] NOT NULL,
	[Version] [int] NULL,
	[AppearsOnPublicRegister] [bit] NULL,
	[LevelId] [int] NULL,
	[TypeId] [int] NULL,
	[SsaId] [int] NULL,
	[GradingTypeId] [int] NULL,
	[GradingScaleId] [int] NULL,
	[PreSixteen] [bit] NULL,
	[SixteenToEighteen] [bit] NULL,
	[EighteenPlus] [bit] NULL,
	[NineteenPlus] [bit] NULL,
	[ImportStatus] [varchar](50) NULL,
	[InsertedTimestamp] [datetime] NULL,
	[EligibleForFunding] BIT NULL, 
    [Name] VARCHAR(250) NULL, 
	[FundedInEngland] BIT NULL,
    CONSTRAINT PK_QualificationVersions PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT FK_AwardingOrganisation FOREIGN KEY (AwardingOrganisationId) REFERENCES [dbo].[AwardingOrganisation] ([Id]),
	CONSTRAINT FK_LifecycleStage FOREIGN KEY (LifecycleStageId) REFERENCES [regulated].[LifecycleStage] ([Id]),
	CONSTRAINT FK_ProcessStatus FOREIGN KEY (ProcessStatusId) REFERENCES [regulated].[ProcessStatus] ([Id]),
	CONSTRAINT FK_Qualification FOREIGN KEY (QualificationId) REFERENCES [dbo].[Qualification] ([Id]),
	CONSTRAINT FK_VersionFieldChanges FOREIGN KEY (VersionFieldChangesId) REFERENCES [regulated].[VersionFieldChanges] ([Id])

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE INDEX IX_QualificationVersions_QualificationId ON [regulated].[QualificationVersions] ([QualificationId]) INCLUDE ([VersionFieldChangesId]);

GO

CREATE INDEX [IX_QualificationVersions_EligibleForFunding] ON [regulated].[QualificationVersions] ([EligibleForFunding])

GO

CREATE INDEX [IX_QualificationVersions_ProcessStatus] ON [regulated].[QualificationVersions] ([ProcessStatusId])

GO

CREATE INDEX IX_QualificationVersions_LifeCycle ON [regulated].[QualificationVersions] (LifecycleStageId)
INCLUDE (QualificationId, ProcessStatusId, AwardingOrganisationId, [Type], Ssa, [Level], [Version],
    PreSixteen, SixteenToEighteen, EighteenPlus, NineteenPlus, OperationalEndDate, CertificationEndDate)

GO

CREATE INDEX [IX_QualificationVersions_AwardingOrganisation] ON [regulated].[QualificationVersions] ([AwardingOrganisationId])

GO

CREATE INDEX [IX_QualificationVersions_Status_Lifecycle_QualificationId_Version] ON [regulated].[QualificationVersions] ( ProcessStatusId, LifecycleStageId, QualificationId, Version DESC ) 
INCLUDE (Id, AwardingOrganisationId, [Level], [Type], SubLevel, Ssa, InsertedTimestamp);

GO

CREATE NONCLUSTERED INDEX [IX_QualificationVersions_EligibleForFunding_Type] ON [regulated].[QualificationVersions] ([EligibleForFunding], [Type])
INCLUDE ([QualificationId], [VersionFieldChangesId], [LifecycleStageId]);


GO

CREATE NONCLUSTERED INDEX [IX_QualificationVersions_LifecycleStage_Reporting]
ON [regulated].[QualificationVersions] ([LifecycleStageId])
INCLUDE (
    [QualificationId],
    [AwardingOrganisationId],
    [Type],
    [Ssa],
    [Level],
    [SubLevel],
    [Version],
    [InsertedTimestamp]
);

GO

CREATE NONCLUSTERED INDEX [IX_QualificationVersions_ProcessStatus_Operational]
ON [regulated].[QualificationVersions] ([ProcessStatusId])
INCLUDE (
    [QualificationId],
    [OperationalEndDate],
    [Version]
);





