CREATE TABLE [regulated].[QaaQualification] (
    [Id] uniqueidentifier NOT NULL,
    [DateOfDataSnapshot] datetime2 NOT NULL,
    [AimCode] varchar(10) NOT NULL,
    [QualificationTitle] varchar(250),
    [AwardingBody] varchar(250) NOT NULL,
    [Level] [varchar](50) NOT NULL,
    [Type] [varchar](50) NOT NULL,
    [Status] [varchar](100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [LastDateForRegistration] datetime2 NOT NULL,
    [LastFundingApprovalEndDate] datetime2 NULL,
    [SectorSubjectArea] varchar(150) NOT NULL,
    CONSTRAINT [PK_QaaQualification] PRIMARY KEY CLUSTERED ([ID] ASC),
    );
GO

CREATE INDEX [IX_QaaQualification_AimCode] ON [regulated].[QaaQualification] ([AimCode])
GO

CREATE INDEX [IX_QaaQualification_QualificationTitle] ON [regulated].[QaaQualification] ([QualificationTitle])
GO