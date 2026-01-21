CREATE TABLE [DefundingLists]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Qan] VARCHAR(100) NOT NULL,
    [Title] NVARCHAR(4000) NULL,
    [AwardingOrganisation] VARCHAR(400) NULL,
    [GuidedLearningHours] NVARCHAR(100) NULL,
    [SectorSubjectArea] NVARCHAR(1000) NULL,
    [RelevantRoute] NVARCHAR(1000) NULL,
    [FundingOffer] NVARCHAR(1000) NULL,
    [InScope] BIT NOT NULL DEFAULT (1),
    [Comments] NVARCHAR(4000) NULL,
    [ImportDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE NONCLUSTERED INDEX IX_DefundingLists_Qan
ON [DefundingLists]([Qan]);
GO
