ALTER TABLE [regulated].[QualificationVersions] DROP CONSTRAINT [FK__Qualifica__Versi__056ECC6A]
GO
ALTER TABLE [regulated].[QualificationVersions] DROP CONSTRAINT [FK__Qualifica__Quali__047AA831]
GO
ALTER TABLE [regulated].[QualificationVersions] DROP CONSTRAINT [FK__Qualifica__Proce__0662F0A3]
GO
ALTER TABLE [regulated].[QualificationVersions] DROP CONSTRAINT [FK__Qualifica__Lifec__075714DC]
GO
ALTER TABLE [regulated].[QualificationVersions] DROP CONSTRAINT [FK__Qualifica__Award__084B3915]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[regulated].[QualificationVersions]') AND type in (N'U'))
DROP TABLE [regulated].[QualificationVersions]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [regulated].[QualificationVersions]  WITH CHECK ADD FOREIGN KEY([AwardingOrganisationId])
REFERENCES [dbo].[AwardingOrganisation] ([Id])
GO
ALTER TABLE [regulated].[QualificationVersions]  WITH CHECK ADD FOREIGN KEY([LifecycleStageId])
REFERENCES [regulated].[LifecycleStage] ([Id])
GO
ALTER TABLE [regulated].[QualificationVersions]  WITH CHECK ADD FOREIGN KEY([ProcessStatusId])
REFERENCES [regulated].[ProcessStatus] ([Id])
GO
ALTER TABLE [regulated].[QualificationVersions]  WITH CHECK ADD FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualification] ([Id])
GO
ALTER TABLE [regulated].[QualificationVersions]  WITH CHECK ADD FOREIGN KEY([VersionFieldChangesId])
REFERENCES [regulated].[VersionFieldChanges] ([Id])
GO
