CREATE TABLE [dbo].[ApplicationReviewFeedbacks](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationReviewId] [uniqueidentifier] NOT NULL,
	[Owner] [nvarchar](4000) NULL,
	[Status] [nvarchar](50) NULL,
	[Comments] [nvarchar](max) NULL,
	[NewMessage] [bit] NULL,
	[Type] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ApplicationReviewFeedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationReviewFeedbacks]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationReviewFeedback_ApplicationReviews] FOREIGN KEY([ApplicationReviewId])
REFERENCES [dbo].[ApplicationReviews] ([Id])
GO

ALTER TABLE [dbo].[ApplicationReviewFeedbacks] CHECK CONSTRAINT [FK_ApplicationReviewFeedback_ApplicationReviews]
GO