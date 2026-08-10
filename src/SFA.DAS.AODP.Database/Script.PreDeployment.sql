/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\MigrationScripts\MVS1_AWARD_855_ConsolidateQaaFundings.sql
:r .\MigrationScripts\MVS2_AWARD_855_RolloverCandidatesPolymorphicSourceBackfill.sql
:r .\MigrationScripts\MVS2_AWARD_1240_FundingExtensionStagingPolymorphicBackfill.sql
