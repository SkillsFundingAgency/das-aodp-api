CREATE TABLE [dbo].[ApplicationReviews](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SharedWithSkillsEngland] [bit] NULL,
	[SharedWithOfqual] [bit] NULL,
 CONSTRAINT [PK_ApplicationReviews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationReviews]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationReviews_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO

ALTER TABLE [dbo].[ApplicationReviews] CHECK CONSTRAINT [FK_ApplicationReviews_Applications]
GO