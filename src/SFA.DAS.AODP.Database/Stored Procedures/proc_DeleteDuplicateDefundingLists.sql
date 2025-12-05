-- ==================================================
-- Author:		Adeel Dar
-- Create date: 05/11/2025
-- Description:	Delete duplicate qualifications 
-- from DefundingLists table and keep the latest one.
-- ==================================================

CREATE PROCEDURE [dbo].[proc_DeleteDuplicateDefundingLists]
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
                    ORDER BY ImportDate DESC, Id DESC
                ) AS rn
            FROM dbo.DefundingLists
            WHERE (@Qan IS NULL OR Qan = @Qan)
        )
        DELETE D
        FROM dbo.DefundingLists D
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

        RAISERROR('proc_DeleteDuplicateDefundingLists failed: %s', 
                  @ErrSeverity, @ErrState, @ErrMsg);
    END CATCH
END
GO