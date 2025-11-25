CREATE TABLE [Pldns]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Qan] VARCHAR(100) NOT NULL,
    [ListUpdatedDate] DATE NULL,
    [Notes] VARCHAR(4000) NULL,

    [PLDNS14-16] DATE NULL,
    [Notes_14-16] VARCHAR(4000) NULL,

    [PLDNS16-19] DATE NULL,
    [Notes16-19] VARCHAR(4000) NULL,

    [LocalFlex] DATE NULL,
    [NotesLocalFlex] VARCHAR(4000) NULL,

    [LegalEntitlementL2-L3] DATE NULL,
    [NotesLegalEntitlementL2-L3] VARCHAR(4000) NULL,

    [LegalEntitlementEngMaths] DATE NULL,
    [NotesLegalEntitlementEngMaths] VARCHAR(4000) NULL,

    [DigitalEntitlement] DATE NULL,
    [NotesDigitalEntitlement] VARCHAR(4000) NULL,

    [ESF-L3-L4] DATE NULL,
    [NotesESF-L3-L4] VARCHAR(4000) NULL,

    [Loans] DATE NULL,
    [NotesLoans] VARCHAR(4000) NULL,

    [LifelongLearningEntitlement] DATE NULL,
    [NotesLifelongLearningEntitlement] VARCHAR(4000) NULL,

    [Level3FreeCoursesForJobs] DATE NULL,
    [NotesLevel 3FreeCoursesForJobs] VARCHAR(4000) NULL,

    [CoF] DATE NULL,
    [NotesCoF] VARCHAR(4000) NULL,

    [StartDate] DATE NULL,
    [NotesStartDate] VARCHAR(4000) NULL,

    [ImportDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO