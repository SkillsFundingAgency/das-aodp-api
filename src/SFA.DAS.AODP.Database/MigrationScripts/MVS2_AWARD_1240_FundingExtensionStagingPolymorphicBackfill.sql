-- Backfills the new polymorphic SourceType/SourceFundingRecordId columns on FundingExtensionStaging
-- from the legacy Ofqual-only QualificationFundingId column, so environments still running the
-- pre-QAA schema don't lose data when this deploys. FundingExtensionStaging is cleared after every
-- funding-extension submit, so this table is expected to be empty in practice, but the backfill is
-- included anyway for consistency and to cover any operation that happens to be in flight at deploy
-- time. Safe to run repeatedly and against environments that never had QualificationFundingId.
--
-- See MVS2_AWARD_855_RolloverCandidatesPolymorphicSourceBackfill.sql for why the column-add and the
-- backfill UPDATE are split into separate batches, and why the UPDATE is built as dynamic SQL.

IF OBJECT_ID(N'[dbo].[FundingExtensionStaging]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'QualificationFundingId') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'SourceType') IS NULL
    ALTER TABLE [dbo].[FundingExtensionStaging] ADD [SourceType] NVARCHAR(50) NULL;
GO

IF OBJECT_ID(N'[dbo].[FundingExtensionStaging]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'QualificationFundingId') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'SourceFundingRecordId') IS NULL
    ALTER TABLE [dbo].[FundingExtensionStaging] ADD [SourceFundingRecordId] UNIQUEIDENTIFIER NULL;
GO

IF OBJECT_ID(N'[dbo].[FundingExtensionStaging]', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'QualificationFundingId') IS NOT NULL
   AND COL_LENGTH(N'dbo.FundingExtensionStaging', N'SourceFundingRecordId') IS NOT NULL
BEGIN
    EXEC(N'
        UPDATE [dbo].[FundingExtensionStaging]
        SET [SourceType] = ''Ofqual'',
            [SourceFundingRecordId] = [QualificationFundingId]
        WHERE [SourceFundingRecordId] IS NULL
          AND [QualificationFundingId] IS NOT NULL;
    ');
END;
GO
