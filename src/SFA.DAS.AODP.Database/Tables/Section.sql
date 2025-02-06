CREATE TABLE [dbo].[Sections](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[FormVersionId] [uniqueidentifier] NOT NULL,
	[Key] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PagesCount] [int] NULL
 CONSTRAINT [PK_Sections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sections]  WITH CHECK ADD  CONSTRAINT [FK_Sections_FormVersions] FOREIGN KEY([FormVersionId])
REFERENCES [dbo].[FormVersions] ([Id])
GO

ALTER TABLE [dbo].[Sections] CHECK CONSTRAINT [FK_Sections_FormVersions]
GO
