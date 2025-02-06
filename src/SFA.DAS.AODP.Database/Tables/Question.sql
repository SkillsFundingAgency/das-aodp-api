CREATE TABLE [dbo].[Questions](
	[Id] [uniqueidentifier] NOT NULL,
	[PageId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Required] [bit] NOT NULL,
	[Order] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Hint] [nvarchar](max) NULL,
	[Key] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Questions] ADD  CONSTRAINT [DF_Questions_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Pages] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([Id])
GO

ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_Pages]
GO


