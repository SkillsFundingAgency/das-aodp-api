CREATE TABLE [dbo].[Routes](
	[Id] [uniqueidentifier] NOT NULL,
	[SourceQuestionId] [uniqueidentifier] NULL,
	[SourceOptionId] [uniqueidentifier] NULL,
	[NextPageId] [uniqueidentifier] NULL,
	[NextSectionId] [uniqueidentifier] NULL,
	[EndSection] [bit] NOT NULL,
	[EndForm] [bit] NOT NULL,
 CONSTRAINT [PK_Routes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Routes]  WITH CHECK ADD  CONSTRAINT [FK_Routes_Pages] FOREIGN KEY([NextPageId])
REFERENCES [dbo].[Pages] ([Id])
GO

ALTER TABLE [dbo].[Routes] CHECK CONSTRAINT [FK_Routes_Pages]
GO

ALTER TABLE [dbo].[Routes]  WITH CHECK ADD  CONSTRAINT [FK_Routes_Questions] FOREIGN KEY([SourceQuestionId])
REFERENCES [dbo].[Questions] ([Id])
GO

ALTER TABLE [dbo].[Routes] CHECK CONSTRAINT [FK_Routes_Questions]
GO

ALTER TABLE [dbo].[Routes]  WITH CHECK ADD  CONSTRAINT [FK_Routes_Sections] FOREIGN KEY([NextSectionId])
REFERENCES [dbo].[Sections] ([Id])
GO

ALTER TABLE [dbo].[Routes] CHECK CONSTRAINT [FK_Routes_Sections]
GO


