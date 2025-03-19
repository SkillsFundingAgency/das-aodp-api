CREATE TABLE [dbo].[ApplicationReviewDecisions](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationReviewId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_ApplicationReviewDecision] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationReviewDecisions]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationReviewDecision_ApplicationReviewDecision] FOREIGN KEY([ApplicationReviewId])
REFERENCES [dbo].[ApplicationReviews] ([Id])
GO

ALTER TABLE [dbo].[ApplicationReviewDecisions] CHECK CONSTRAINT [FK_ApplicationReviewDecision_ApplicationReviewDecision]
GO
