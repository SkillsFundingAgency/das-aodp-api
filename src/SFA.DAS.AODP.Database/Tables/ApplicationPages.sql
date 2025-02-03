CREATE TABLE [dbo].[ApplicationPages](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[PageId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_ApplicationPages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationPages]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationPages_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO

ALTER TABLE [dbo].[ApplicationPages] CHECK CONSTRAINT [FK_ApplicationPages_Applications]
GO

ALTER TABLE [dbo].[ApplicationPages]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationPages_Pages] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([Id])
GO

ALTER TABLE [dbo].[ApplicationPages] CHECK CONSTRAINT [FK_ApplicationPages_Pages]
GO
