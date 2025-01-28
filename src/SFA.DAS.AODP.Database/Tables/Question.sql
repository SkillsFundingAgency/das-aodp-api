CREATE TABLE [dbo].[Question](
	[Id] [uniqueidentifier] NOT NULL,
	[PageId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Required] [bit] NOT NULL,
	[Order] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Hint] [nvarchar](max) NULL,
	[MultiChoice] [nvarchar](max) NULL,
	[TextValidator] [nvarchar](max) NULL,
	[IntegerValidator] [nvarchar](max) NULL,
	[DecimalValidator] [nvarchar](max) NULL,
	[DateValidator] [nvarchar](max) NULL,
	[MultiChoiceValidator] [nvarchar](max) NULL,
	[BooleanValidaor] [nvarchar](max) NULL,
	[RadioValidator] [nvarchar](max) NULL,
	[Key] [uniqueidentifier] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Questions_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Pages] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([Id])
GO

ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Questions_Pages]
GO


