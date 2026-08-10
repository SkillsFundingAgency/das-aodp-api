-- Backfills the new polymorphic SourceType/SourceQualificationId columns on RolloverCandidates
-- and RolloverWorkflowCandidate from the legacy Ofqual-only QualificationVersionId column, so
-- that environments still running the pre-QAA schema don't lose data when this deploys.
-- Safe to run repeatedly and against environments that never had QualificationVersionId
-- (including environments where that column, or the table itself, has never existed).
--
-- Dynamic SQL is used deliberately: SQL Server binds column names for an entire batch at parse
-- time, before any statement in it executes. A plain UPDATE referencing QualificationVersionId
-- (or a column just added earlier in the same batch) fails to compile on an environment where
-- that column doesn't exist, even inside an IF branch that would never run. Building the
-- statement as a string and executing it via EXEC defers parsing until the guard has already
-- confirmed the column exists.

IF OBJECT_ID(N'[dbo].[RolloverCandidates]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'SourceType') IS NULL
    ALTER TABLE [dbo].[RolloverCandidates] ADD [SourceType] NVARCHAR(50) NULL;
GO

IF OBJECT_ID(N'[dbo].[RolloverCandidates]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'SourceQualificationId') IS NULL
    ALTER TABLE [dbo].[RolloverCandidates] ADD [SourceQualificationId] UNIQUEIDENTIFIER NULL;
GO

IF OBJECT_ID(N'[dbo].[RolloverCandidates]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverCandidates', N'SourceQualificationId') IS NOT NULL
BEGIN
    EXEC(N'
        UPDATE [dbo].[RolloverCandidates]
        SET [SourceType] = ''Ofqual'',
            [SourceQualificationId] = [QualificationVersionId]
        WHERE [SourceQualificationId] IS NULL
          AND [QualificationVersionId] IS NOT NULL;
    ');
END;
GO

IF OBJECT_ID(N'[dbo].[RolloverWorkflowCandidate]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'SourceType') IS NULL
    ALTER TABLE [dbo].[RolloverWorkflowCandidate] ADD [SourceType] NVARCHAR(50) NULL;
GO

IF OBJECT_ID(N'[dbo].[RolloverWorkflowCandidate]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'SourceQualificationId') IS NULL
    ALTER TABLE [dbo].[RolloverWorkflowCandidate] ADD [SourceQualificationId] UNIQUEIDENTIFIER NULL;
GO

IF OBJECT_ID(N'[dbo].[RolloverWorkflowCandidate]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'QualificationVersionId') IS NOT NULL
   AND COL_LENGTH(N'dbo.RolloverWorkflowCandidate', N'SourceQualificationId') IS NOT NULL
BEGIN
    EXEC(N'
        UPDATE [dbo].[RolloverWorkflowCandidate]
        SET [SourceType] = ''Ofqual'',
            [SourceQualificationId] = [QualificationVersionId]
        WHERE [SourceQualificationId] IS NULL
          AND [QualificationVersionId] IS NOT NULL;
    ');
END;
GO
