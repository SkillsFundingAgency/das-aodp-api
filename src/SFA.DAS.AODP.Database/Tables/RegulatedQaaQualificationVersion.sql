CREATE TABLE [regulated].[QaaQualificationVersion] (
    [Id] uniqueidentifier NOT NULL,
    [QaaQualificationId] uniqueidentifier NOT NULL,
    [AimCode] varchar(10) NOT NULL,
    [ChangeVersion] bigint NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    [ContentHash] varchar(64) NOT NULL,
    [QualificationTitle] varchar(250) NULL,
    [AwardingBody] varchar(250) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [LastDateForRegistration] datetime2 NOT NULL,
    [IsDiscontinued] bit NOT NULL,
    [DiscontinuedDate] datetime2 NULL,
    [SectorSubjectArea] varchar(150) NOT NULL,
    [LastDateForRegistrationChangeType] varchar(50) NOT NULL,
    CONSTRAINT [PK_QaaQualificationVersion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QaaQualificationVersion_QaaQualification] FOREIGN KEY ([QaaQualificationId])
        REFERENCES [regulated].[QaaQualification] ([Id])
);
GO

CREATE INDEX [IX_QaaQualificationVersion_QaaQualificationId] ON [regulated].[QaaQualificationVersion] ([QaaQualificationId])
GO

CREATE INDEX [IX_QaaQualificationVersion_AimCode] ON [regulated].[QaaQualificationVersion] ([AimCode])
GO

CREATE INDEX [IX_QaaQualificationVersion_ChangeVersion] ON [regulated].[QaaQualificationVersion] ([ChangeVersion])
GO

CREATE INDEX [IX_QaaQualificationVersion_ChangedAt] ON [regulated].[QaaQualificationVersion] ([ChangedAt])
GO

CREATE INDEX [IX_QaaQualificationVersion_LastDateForRegistrationChangeType] ON [regulated].[QaaQualificationVersion] ([LastDateForRegistrationChangeType])
GO
