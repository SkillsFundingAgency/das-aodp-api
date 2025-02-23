CREATE TABLE [dbo].[QualificationDiscussionHistory](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[QualificationId] [uniqueidentifier] NOT NULL,
	[ActionTypeId] [uniqueidentifier] NOT NULL,
	[UserDisplayName] [varchar](250) NULL,
	[Notes] [varchar](max) NULL,
	[Timestamp] [datetime] NULL, 
    CONSTRAINT [PK_QualificationDiscussionHistory] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ActionType] FOREIGN KEY (ActionTypeId) REFERENCES dbo.ActionType(Id),
	CONSTRAINT [FK_Qualification] FOREIGN KEY (QualificationId) REFERENCES dbo.Qualification(Id),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

