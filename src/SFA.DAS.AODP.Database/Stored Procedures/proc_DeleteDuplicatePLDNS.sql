-- ==================================================
-- Author:		Adeel Dar
-- Create date: 24/11/2025
-- Description:	Delete duplicate qualifications 
-- from PLDNS table and keep the latest one (by Id) per Qan.
-- ==================================================

CREATE PROCEDURE [dbo].[proc_DeleteDuplicatePLDNS]
	@Qan VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        ;WITH Ranked AS
        (
            SELECT 
                Id,
                Qan,
                ImportDate,
                ROW_NUMBER() OVER (
                    PARTITION BY Qan
                    ORDER BY Id DESC
                ) AS rn
            FROM dbo.PLDNS
            WHERE (@Qan IS NULL OR Qan = @Qan)
        )
        DELETE D
        FROM dbo.PLDNS D
        INNER JOIN Ranked R ON D.Id = R.Id
        WHERE R.rn > 1;

        DECLARE @DeletedRows INT = @@ROWCOUNT;

        COMMIT TRAN;

        -- Return number of deleted rows
        SELECT @DeletedRows AS DeletedRows;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN;

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();

        RAISERROR('proc_DeleteDuplicatePLDNS failed: %s', 
                  @ErrSeverity, @ErrState, @ErrMsg);
    END CATCH
END
GO