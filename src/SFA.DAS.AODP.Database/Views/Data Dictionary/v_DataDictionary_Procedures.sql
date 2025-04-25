Create View [dbo].[v_DataDictionary_Procedures] AS

SELECT 
    p.SPECIFIC_SCHEMA AS [Procedure Schema],
    p.SPECIFIC_NAME AS [Procedure Name],
    rf.referenced_schema_name AS [Referenced Schema],
    rf.referenced_entity_name AS [Referenced Table]
FROM 
    INFORMATION_SCHEMA.ROUTINES p
LEFT JOIN 
    sys.sql_expression_dependencies rf 
    ON OBJECT_ID(p.SPECIFIC_NAME) = rf.referencing_id
WHERE 
    p.ROUTINE_TYPE = 'PROCEDURE'
GO