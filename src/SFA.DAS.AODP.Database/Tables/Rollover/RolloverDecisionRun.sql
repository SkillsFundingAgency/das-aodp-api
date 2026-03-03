CREATE TABLE [dbo].[RolloverDecisionRun] (
    [Id]                                     UNIQUEIDENTIFIER NOT NULL,
    [AcademicYear]                           NVARCHAR (255)   NOT NULL,
    [SelectionMethod]                        NVARCHAR (255)   NOT NULL,
    [FundingEndDateEligibilityThreshold]     DATETIME2 (7)    NULL,
    [OperationalEndDateEligibilityThreshold] DATETIME2 (7)    NULL,
    [MaximumApprovalFundingEndDate]          DATETIME2 (7)    NULL,
    [CreatedAt]                              DATETIME2 (7)    NOT NULL,
    [CreatedByUsername]                      NVARCHAR (255)   NOT NULL,
    [CompletedAt]                            DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RolloverDecisionRun_Year]
    ON [dbo].[RolloverDecisionRun]([AcademicYear] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RolloverDecisionRun_YearCreatedAt]
    ON [dbo].[RolloverDecisionRun]([AcademicYear] ASC, [CreatedAt] ASC);

