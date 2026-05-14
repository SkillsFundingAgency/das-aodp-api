CREATE TABLE [regulated].[QaaQualification] (
    [Id] uniqueidentifier NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [ChangeVersion] bigint NOT NULL DEFAULT 1,
    [LastChangedAt] datetime2 NOT NULL DEFAULT GETDATE(),
    [ContentHash] varchar(64) NOT NULL DEFAULT '',
    [LatestImportComparisonOutcome] varchar(50) NOT NULL DEFAULT 'Unchanged',
    [PublicationStatus] varchar(50) NOT NULL DEFAULT 'Published',
    [LastDateForRegistrationChangeType] varchar(50) NOT NULL DEFAULT 'NotChanged',
    [IsRegistrationDateExtended] bit NOT NULL DEFAULT 0,
    [IsRegistrationDateBroughtForward] bit NOT NULL DEFAULT 0,
    [AimCode] varchar(10) NOT NULL,
    [QualificationTitle] varchar(250),
    [AwardingBody] varchar(250) NOT NULL,
    [Level] [varchar](50) NOT NULL,
    [Type] [varchar](50) NOT NULL,
    [Status] [varchar](100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [LastDateForRegistration] datetime2 NOT NULL,
    [IsDiscontinued] bit NOT NULL DEFAULT 0,
    [DiscontinuedDate] datetime2 NULL,
    [LastFundingApprovalEndDate] datetime2 NULL,
    [LastPublishedAt] datetime2 NULL,
    [LastPublishedChangeVersion] bigint NULL,
    [SectorSubjectArea] varchar(150) NOT NULL,
    CONSTRAINT [PK_QaaQualification] PRIMARY KEY CLUSTERED ([ID] ASC)
    );
GO

CREATE UNIQUE INDEX [IX_QaaQualification_AimCode] ON [regulated].[QaaQualification] ([AimCode])
GO

CREATE INDEX [IX_QaaQualification_QualificationTitle] ON [regulated].[QaaQualification] ([QualificationTitle])
GO

CREATE INDEX [IX_QaaQualification_ChangeVersion] ON [regulated].[QaaQualification] ([ChangeVersion])
GO

CREATE INDEX [IX_QaaQualification_DateOfDataSnapshot] ON [regulated].[QaaQualification] ([DateOfDataSnapshot])
GO

CREATE INDEX [IX_QaaQualification_LastDateForRegistration] ON [regulated].[QaaQualification] ([LastDateForRegistration])
GO

CREATE INDEX [IX_QaaQualification_IsDiscontinued] ON [regulated].[QaaQualification] ([IsDiscontinued])
GO

CREATE INDEX [IX_QaaQualification_LatestImportComparisonOutcome] ON [regulated].[QaaQualification] ([LatestImportComparisonOutcome])
GO

CREATE INDEX [IX_QaaQualification_PublicationStatus] ON [regulated].[QaaQualification] ([PublicationStatus])
GO

CREATE INDEX [IX_QaaQualification_LastDateForRegistrationChangeType] ON [regulated].[QaaQualification] ([LastDateForRegistrationChangeType])
GO

CREATE INDEX [IX_QaaQualification_IsRegistrationDateExtended] ON [regulated].[QaaQualification] ([IsRegistrationDateExtended])
GO

CREATE INDEX [IX_QaaQualification_IsRegistrationDateBroughtForward] ON [regulated].[QaaQualification] ([IsRegistrationDateBroughtForward])
GO

CREATE INDEX [IX_QaaQualification_DiscontinuedDate] ON [regulated].[QaaQualification] ([DiscontinuedDate])
GO
