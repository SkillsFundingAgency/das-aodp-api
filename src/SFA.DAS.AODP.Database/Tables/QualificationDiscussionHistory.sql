CREATE TABLE [dbo].[QualificationDiscussionHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[ActionTypeId] [uniqueidentifier] NOT NULL,
	[UserDisplayName] [varchar](250) NULL,
	[Notes] [varchar](max) NULL,
	[Timestamp] [datetime] NULL,
	[Title] VARCHAR(250) NULL, 
    CONSTRAINT PK_QualificationDiscussionHistory PRIMARY KEY CLUSTERED (ID ASC),
	CONSTRAINT FK_ActionType FOREIGN KEY (ActionTypeID) REFERENCES [dbo].[ActionType] ([Id]),
	CONSTRAINT FK_Qualification FOREIGN KEY (QualificationId) REFERENCES [dbo].[Qualification] (Id)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO