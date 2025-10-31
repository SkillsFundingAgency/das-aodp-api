CREATE TABLE [funded].[Qualifications](
	[Id] [uniqueidentifier] NOT NULL,
	[DateOfOfqualDataSnapshot] [datetime] NULL,
	[QualificationId] [uniqueidentifier] NULL,
	[AwardingOrganisationId] [uniqueidentifier] NULL,
	[Level] [nvarchar](255) NULL,
	[QualificationType] [nvarchar](255) NULL,
	[SubCategory] [nvarchar](255) NULL,
	[SectorSubjectArea] [nvarchar](255) NULL,
	[Status] [nvarchar](255) NULL,
	[AwardingOrganisationUrl] [nvarchar](255) NULL,
	[ImportDate] [datetime] NOT NULL,
	CONSTRAINT PK_Qualifications PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT FK_AwardingOrganisation FOREIGN KEY (AwardingOrganisationId) REFERENCES dbo.AwardingOrganisation (Id),
	CONSTRAINT FK_Qualification FOREIGN KEY (QualificationId) REFERENCES dbo.Qualification (Id)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Qualifications_QualificationId]
ON [funded].[Qualifications] ([QualificationId]);
