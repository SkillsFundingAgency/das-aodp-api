ALTER TABLE [dbo].[QualificationDiscussionHistory] DROP CONSTRAINT [FK__Qualifica__Quali__0B27A5C0]
GO
ALTER TABLE [dbo].[QualificationDiscussionHistory] DROP CONSTRAINT [FK__Qualifica__Actio__0C1BC9F9]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QualificationDiscussionHistory]') AND type in (N'U'))
DROP TABLE [dbo].[QualificationDiscussionHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualificationDiscussionHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[ActionTypeId] [uniqueidentifier] NOT NULL,
	[UserDisplayName] [varchar](250) NULL,
	[Notes] [varchar](max) NULL,
	[Timestamp] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[QualificationDiscussionHistory]  WITH CHECK ADD FOREIGN KEY([ActionTypeId])
REFERENCES [dbo].[ActionType] ([Id])
GO
ALTER TABLE [dbo].[QualificationDiscussionHistory]  WITH CHECK ADD FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualification] ([Id])
GO
