ALTER TABLE [funded].[QualificationOffers] DROP CONSTRAINT [FK__Qualifica__Quali__019E3B86]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[funded].[QualificationOffers]') AND type in (N'U'))
DROP TABLE [funded].[QualificationOffers]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [funded].[QualificationOffers](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NULL,
	[Notes] [varchar](max) NULL,
	[FundingAvailable] [bit] NULL,
	[FundingApprovalStartDate] [datetime] NULL,
	[FundingApprovalEndDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [funded].[QualificationOffers]  WITH CHECK ADD FOREIGN KEY([QualificationId])
REFERENCES [funded].[Qualifications] ([Id])
GO
