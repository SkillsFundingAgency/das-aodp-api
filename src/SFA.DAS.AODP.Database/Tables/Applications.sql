
CREATE TABLE [dbo].[Applications](
	[Id] [uniqueidentifier] NOT NULL,
	[FormVersionId] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Owner] NVARCHAR(4000) NOT NULL, 
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_FormVersions] FOREIGN KEY([FormVersionId])
REFERENCES [dbo].[FormVersions] ([Id])
GO

ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_FormVersions]
GO
