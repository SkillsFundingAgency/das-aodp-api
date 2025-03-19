CREATE TABLE [dbo].[ApplicationReviewFundings](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationReviewId] [uniqueidentifier] NOT NULL,
	[FundingOfferId] [uniqueidentifier] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_ApplicationReviewFundings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationReviewFundings]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationReviewFundings_ApplicationReviews] FOREIGN KEY([ApplicationReviewId])
REFERENCES [dbo].[ApplicationReviews] ([Id])
GO

ALTER TABLE [dbo].[ApplicationReviewFundings] CHECK CONSTRAINT [FK_ApplicationReviewFundings_ApplicationReviews]
GO

ALTER TABLE [dbo].[ApplicationReviewFundings]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationReviewFundings_FundingOffers] FOREIGN KEY([FundingOfferId])
REFERENCES [dbo].[FundingOffers] ([Id])
GO

ALTER TABLE [dbo].[ApplicationReviewFundings] CHECK CONSTRAINT [FK_ApplicationReviewFundings_FundingOffers]
GO