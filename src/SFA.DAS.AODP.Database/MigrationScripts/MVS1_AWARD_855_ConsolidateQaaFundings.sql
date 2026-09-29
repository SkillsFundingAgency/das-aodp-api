IF OBJECT_ID(N'[funded].[QaaQualificationFundings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'funded.QaaQualificationFundings', N'AcademicYear') IS NOT NULL
BEGIN
    ;WITH RankedFundings AS
    (
        SELECT
            Id,
            ROW_NUMBER() OVER
            (
                PARTITION BY QaaQualificationId, FundingOfferId
                ORDER BY UpdatedAt DESC, CreatedAt DESC, Id DESC
            ) AS RowNumber
        FROM [funded].[QaaQualificationFundings]
    )
    DELETE FROM RankedFundings
    WHERE RowNumber > 1;
END;
GO
