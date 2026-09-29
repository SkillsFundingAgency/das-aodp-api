-- Pre-deployment script: drop objects blocking column changes on rollback deploy
-- Safe to run even if objects don't exist (IF EXISTS guards)

IF DB_NAME() IN ('das-at-aodp-db', 'das-test-aodp-db')
BEGIN

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_RolloverCandidates_QualOfferYearRound')
    DROP INDEX UX_RolloverCandidates_QualOfferYearRound ON dbo.RolloverCandidates;

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_RolloverCandidates_QualOfferYearRound')
    ALTER TABLE dbo.RolloverCandidates DROP CONSTRAINT UQ_RolloverCandidates_QualOfferYearRound;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FundingExtensionStaging_QualificationFundingId')
    DROP INDEX IX_FundingExtensionStaging_QualificationFundingId ON dbo.FundingExtensionStaging;

END