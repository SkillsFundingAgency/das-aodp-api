﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\MigrationScripts\MVS1_Jobs_Data_Seed.sql
:r .\MigrationScripts\MVS1_Qual_Ref_Data.sql
:r .\MigrationScripts\MVS1_Qual_Ref_Data_award_185.sql
:r .\MigrationScripts\MVS1_AWARD452_Jobs.sql
:r .\MigrationScripts\MVS1_AWARD_633_NewLifeCycle.sql
:r .\MigrationScripts\MVS1_AWARD_634_OfferTypes.sql