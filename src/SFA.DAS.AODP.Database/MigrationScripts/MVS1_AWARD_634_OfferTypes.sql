BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_AWARD_634_OfferTypes NVARCHAR(100) = 'MVS1_AWARD_634_OfferTypes'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_AWARD_634_OfferTypes)
        BEGIN

            PRINT @ScriptName_MVS1_AWARD_634_OfferTypes + ' has already been run.'

        END
    ELSE

    BEGIN        
              
       INSERT INTO [dbo].[FundingOffers] ([Id], [Name])
        VALUES
           ('00000000-0000-0000-0000-000000000001', 'LegalEntitlementEnglishandMaths'),
			('00000000-0000-0000-0000-000000000002','LifelongLearningEntitlement'),
			('00000000-0000-0000-0000-000000000003','LocalFlexibilities'),
			('00000000-0000-0000-0000-000000000004','DigitalEntitlement'),
			('00000000-0000-0000-0000-000000000005','LegalEntitlementL2L3'),
			('00000000-0000-0000-0000-000000000006','AdvancedLearnerLoans'),
			('00000000-0000-0000-0000-000000000007','Age1619'),
			('00000000-0000-0000-0000-000000000008','L3FreeCoursesForJobs'),
			('00000000-0000-0000-0000-000000000009','Age1416')     		          

        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_AWARD_634_OfferTypes,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH