CREATE TABLE [dbo].[ApplicationQuestionAnswers](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationPageId] [uniqueidentifier] NOT NULL,
	[QuestionId] [uniqueidentifier] NOT NULL,
	[TextValue] [nvarchar](max) NULL,
	[DateValue] DATE NULL,
	[OptionsValue] [nvarchar](max) NULL,
	[NumberValue] decimal(18, 2) NULL
 CONSTRAINT [PK_ApplicationQuestionAnswers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationQuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationQuestionAnswers_ApplicationPages] FOREIGN KEY([ApplicationPageId])
REFERENCES [dbo].[ApplicationPages] ([Id])
GO

ALTER TABLE [dbo].[ApplicationQuestionAnswers] CHECK CONSTRAINT [FK_ApplicationQuestionAnswers_ApplicationPages]
GO

ALTER TABLE [dbo].[ApplicationQuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationQuestionAnswers_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([Id])
GO

ALTER TABLE [dbo].[ApplicationQuestionAnswers] CHECK CONSTRAINT [FK_ApplicationQuestionAnswers_Questions]
GO


