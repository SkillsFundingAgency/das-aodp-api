CREATE TABLE [dbo].[FormVersion](
	[Id] [uniqueidentifier] NOT NULL,
	[FormId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Version] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Order] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FormVersion] ADD  CONSTRAINT [DF_FormVersion_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[FormVersion]  WITH CHECK ADD  CONSTRAINT [FK_FormVersion_Form] FOREIGN KEY([FormId])
REFERENCES [dbo].[Form] ([Id])
GO

ALTER TABLE [dbo].[FormVersion] CHECK CONSTRAINT [FK_FormVersion_Form]
GO
