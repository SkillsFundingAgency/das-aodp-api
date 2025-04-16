CREATE TABLE [dbo].[Messages] (
    [Id] [uniqueidentifier] NOT NULL,
    [ApplicationId] [uniqueidentifier] NOT NULL,
    [Text] [nvarchar](max) NOT NULL,
    [Type] [nvarchar](4000) NOT NULL,
    [MessageHeader] [nvarchar](4000) NOT NULL,
    [SharedWithDfe] [bit] NOT NULL,
    [SharedWithOfqual] [bit] NOT NULL,
    [SharedWithSkillsEngland] [bit] NOT NULL,
    [SharedWithAwardingOrganisation] [bit] NOT NULL,
    [SentAt] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [SentByName] [nvarchar](255) NOT NULL,
    [SentByEmail] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Messages] WITH CHECK ADD CONSTRAINT [FK_Messages_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO

ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Applications]
GO
