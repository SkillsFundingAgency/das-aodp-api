Create View [dbo].[v_DataDictionary_Views] AS
SELECT 
    c.TABLE_SCHEMA AS [Schema],
    c.TABLE_NAME AS [View Name],
    c.COLUMN_NAME AS [Column Name],
    c.DATA_TYPE AS [Data Type],
    c.CHARACTER_MAXIMUM_LENGTH AS [Max Length],
    c.IS_NULLABLE AS [Is Nullable],
    rf.referenced_schema_name AS [Referenced Schema],
    rf.referenced_entity_name AS [Referenced Table]
FROM 
    INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN 
    sys.sql_expression_dependencies rf 
    ON OBJECT_ID(c.TABLE_NAME) = rf.referencing_id
WHERE 
    c.TABLE_NAME IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS)

GO