CREATE TABLE [regulated].[QaaQualificationHistory] (
    [Id] uniqueidentifier NOT NULL,
    [QaaQualificationId] uniqueidentifier NOT NULL,
    [AimCode] nvarchar(10) NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    [QualificationTitle] nvarchar(250) NULL,
    [AwardingBody] nvarchar(250) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [LastDateForRegistration] datetime2 NOT NULL,
    [IsDiscontinued] bit NOT NULL,
    [DiscontinuedDate] datetime2 NULL,
    [SectorSubjectArea] nvarchar(150) NOT NULL,
    [LastDateForRegistrationChangeType] nvarchar(50) NOT NULL,
    [Age1619FundingApprovalEndDate] date NULL,
    [AdvancedLearnerLoansFundingApprovalEndDate] date NULL,
    [LegalEntitlementL2L3FundingApprovalEndDate] date NULL,
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
