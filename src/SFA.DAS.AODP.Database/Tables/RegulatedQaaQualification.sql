CREATE TABLE [regulated].[QaaQualification] (
    [Id] uniqueidentifier NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [AimCode] nvarchar(10) NOT NULL,
    [QualificationTitle] nvarchar(250),
    [AwardingBody] nvarchar(250) NOT NULL,
    [Level] [nvarchar](50) NOT NULL,
    [Type] [nvarchar](50) NOT NULL,
    [Status] [nvarchar](100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [LastDateForRegistration] datetime2 NOT NULL,
    [LastFundingApprovalEndDate] datetime2 NULL,
    [SectorSubjectArea] nvarchar(150) NOT NULL,
    CONSTRAINT [PK_QaaQualification] PRIMARY KEY CLUSTERED ([ID] ASC),
    );
GO

CREATE INDEX [IX_QaaQualification_AimCode] ON [regulated].[QaaQualification] ([AimCode])
GO

CREATE INDEX [IX_QaaQualification_QualificationTitle] ON [regulated].[QaaQualification] ([QualificationTitle])
GO