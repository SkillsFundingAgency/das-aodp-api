CREATE TABLE [dbo].[QaaQualificationDiscussionHistory] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [QaaQualificationId] UNIQUEIDENTIFIER NOT NULL,
    [ActionTypeId]      UNIQUEIDENTIFIER NOT NULL,
    [UserDisplayName]   VARCHAR(250) NULL,
    [Notes]             VARCHAR(MAX) NULL,
    [Timestamp]         DATETIME NULL,
    [Title]             VARCHAR(250) NULL,
    CONSTRAINT [PK_QaaQualificationDiscussionHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QaaQualificationDiscussionHistory_ActionType] FOREIGN KEY ([ActionTypeId]) REFERENCES [dbo].[ActionType] ([Id]),
    CONSTRAINT [FK_QaaQualificationDiscussionHistory_QaaQualification] FOREIGN KEY ([QaaQualificationId]) REFERENCES [regulated].[QaaQualification] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_QaaQualificationDiscussionHistory_QaaQualificationId]
    ON [dbo].[QaaQualificationDiscussionHistory]([QaaQualificationId] ASC);
GO
