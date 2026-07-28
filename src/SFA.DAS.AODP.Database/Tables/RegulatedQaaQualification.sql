CREATE TABLE [regulated].[QaaQualification] (
    [Id] uniqueidentifier NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [FirstSeenAt] datetime2 NOT NULL DEFAULT GETDATE(),
    [LastChangedAt] datetime2 NOT NULL DEFAULT GETDATE(),
    [LatestImportComparisonOutcome] nvarchar(50) NOT NULL DEFAULT 'Unchanged',
    [LastDateForRegistrationChangeType] nvarchar(50) NOT NULL DEFAULT 'NotChanged',
    [LatestQaaQualificationHistoryId] uniqueidentifier NULL,
    [AimCode] nvarchar(10) NOT NULL,
    [QualificationTitle] nvarchar(250),
    [AwardingBody] nvarchar(250) NOT NULL,
    [Level] [nvarchar](50) NOT NULL,
    [Type] [nvarchar](50) NOT NULL,
    [Status] [nvarchar](100) NOT NULL,
    [StartDate] date NOT NULL,
    [LastDateForRegistration] date NOT NULL,
    [IsDiscontinued] bit NOT NULL DEFAULT 0,
    [DiscontinuedDate] date NULL,
    [Age1619FundingApprovalEndDate] date NULL,
    [AdvancedLearnerLoansFundingApprovalEndDate] date NULL,
    [LegalEntitlementL2L3FundingApprovalEndDate] date NULL,
    [SectorSubjectArea] nvarchar(150) NOT NULL,
    CONSTRAINT [PK_QaaQualification] PRIMARY KEY CLUSTERED ([ID] ASC)
    );
GO

CREATE UNIQUE INDEX [IX_QaaQualification_AimCode] ON [regulated].[QaaQualification] ([AimCode])
GO

CREATE INDEX [IX_QaaQualification_QualificationTitle] ON [regulated].[QaaQualification] ([QualificationTitle])
GO

CREATE INDEX [IX_QaaQualification_LatestQaaQualificationHistoryId] ON [regulated].[QaaQualification] ([LatestQaaQualificationHistoryId])
GO