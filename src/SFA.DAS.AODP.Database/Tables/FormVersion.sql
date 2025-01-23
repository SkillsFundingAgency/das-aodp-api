﻿CREATE TABLE [dbo].[FormVersions](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[FormId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Version] DATETIME NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Order] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL, 
    CONSTRAINT [PK_FormVersions] PRIMARY KEY ([Id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FormVersions]  WITH CHECK ADD  CONSTRAINT [FK_FormVersion_Form] FOREIGN KEY([FormId])
REFERENCES [dbo].[Forms] ([Id])
GO

ALTER TABLE [dbo].[FormVersion] CHECK CONSTRAINT [FK_FormVersion_Form]
GO
