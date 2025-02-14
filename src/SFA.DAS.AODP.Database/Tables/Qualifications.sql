ALTER TABLE [funded].[Qualifications] DROP CONSTRAINT [FK__Qualifica__Quali__7EC1CEDB]
GO
ALTER TABLE [funded].[Qualifications] DROP CONSTRAINT [FK__Qualifica__Award__7FB5F314]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[funded].[Qualifications]') AND type in (N'U'))
DROP TABLE [funded].[Qualifications]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [funded].[Qualifications](
	[Id] [uniqueidentifier] NOT NULL,
	[DateOfOfqualDataSnapshot] [datetime] NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[AwardingOrganisationId] [uniqueidentifier] NOT NULL,
	[Level] [nvarchar](255) NULL,
	[QualificationType] [nvarchar](255) NULL,
	[SubCategory] [nvarchar](255) NULL,
	[SectorSubjectArea] [nvarchar](255) NULL,
	[Status] [nvarchar](255) NULL,
	[AwardingOrganisationUrl] [nvarchar](255) NULL,
	[ImportDate] [datetime] NOT NULL,
 CONSTRAINT [PK_qualifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [funded].[Qualifications]  WITH CHECK ADD FOREIGN KEY([AwardingOrganisationId])
REFERENCES [dbo].[AwardingOrganisation] ([Id])
GO
ALTER TABLE [funded].[Qualifications]  WITH CHECK ADD FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualification] ([Id])
GO
