CREATE TABLE [regulated].[QaaQualificationHistory] (
    [Id] uniqueidentifier NOT NULL,
    [QaaQualificationId] uniqueidentifier NOT NULL,
    [AimCode] varchar(10) NOT NULL,
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
    CONSTRAINT [PK_QaaQualificationHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QaaQualificationHistory_QaaQualification] FOREIGN KEY ([QaaQualificationId])
        REFERENCES [regulated].[QaaQualification] ([Id])
);
GO

CREATE INDEX [IX_QaaQualificationHistory_QaaQualificationId] ON [regulated].[QaaQualificationHistory] ([QaaQualificationId])
GO

CREATE INDEX [IX_QaaQualificationHistory_AimCode] ON [regulated].[QaaQualificationHistory] ([AimCode])
GO

CREATE INDEX [IX_QaaQualificationHistory_ChangedAt] ON [regulated].[QaaQualificationHistory] ([ChangedAt])
GO

CREATE INDEX [IX_QaaQualificationHistory_LastDateForRegistrationChangeType] ON [regulated].[QaaQualificationHistory] ([LastDateForRegistrationChangeType])
GO
