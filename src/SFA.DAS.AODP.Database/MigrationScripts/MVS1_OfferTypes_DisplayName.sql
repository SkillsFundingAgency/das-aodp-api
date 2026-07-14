BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_OfferTypes_DisplayName NVARCHAR(100) = 'MVS1_OfferTypes_DisplayName'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_OfferTypes_DisplayName)
        BEGIN

            PRINT @ScriptName_MVS1_OfferTypes_DisplayName + ' has already been run.'

        END
    ELSE

    BEGIN

        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Legal Entitlement English andMaths' WHERE [Id] = '00000000-0000-0000-0000-000000000001';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Lifelong Learning Entitlement' WHERE [Id] = '00000000-0000-0000-0000-000000000002';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Local Flexibilities' WHERE [Id] = '00000000-0000-0000-0000-000000000003';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Digital Entitlement' WHERE [Id] = '00000000-0000-0000-0000-000000000004';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Legal Entitlement L2/L3' WHERE [Id] = '00000000-0000-0000-0000-000000000005';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Advanced Learner Loans' WHERE [Id] = '00000000-0000-0000-0000-000000000006';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Age 16-19' WHERE [Id] = '00000000-0000-0000-0000-000000000007';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Free Courses for Jobs' WHERE [Id] = '00000000-0000-0000-0000-000000000008';
        UPDATE [dbo].[FundingOffers] SET [DisplayName] = 'Age 14-16' WHERE [Id] = '00000000-0000-0000-0000-000000000009';

        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_OfferTypes_DisplayName,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH