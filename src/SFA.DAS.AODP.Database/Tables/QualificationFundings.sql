CREATE TABLE [funded].[QualificationFundings](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationVersionId] [uniqueidentifier] NOT NULL,
	[FundingOfferId] [uniqueidentifier] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_QualificationFundings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [funded].[QualificationFundings]  WITH CHECK ADD  CONSTRAINT [FK_QualificationFundings_QualificationVersions] FOREIGN KEY([QualificationVersionId])
REFERENCES [regulated].[QualificationVersions] ([Id])
GO

ALTER TABLE [funded].[QualificationFundings] CHECK CONSTRAINT [FK_QualificationFundings_QualificationVersions]
GO

ALTER TABLE [funded].[QualificationFundings]  WITH CHECK ADD  CONSTRAINT [FK_QualificationFundings_FundingOffers] FOREIGN KEY([FundingOfferId])
REFERENCES [dbo].[FundingOffers] ([Id])
GO

ALTER TABLE [funded].[QualificationFundings] CHECK CONSTRAINT [FK_QualificationFundings_FundingOffers]
GO

CREATE INDEX [IX_QualificationFundings_QualificationVersion] ON [funded].[QualificationFundings] ([QualificationVersionId])
GO

CREATE INDEX [IX_QualificationFunding_Version_Offer] ON [funded].[QualificationFundings] (QualificationVersionId, FundingOfferId)
INCLUDE (StartDate, EndDate)

