Create view [dbo].[v_DataDictionary_Indexes] AS

SELECT 
    t.name AS [Table Name],
    i.name AS [Index Name],
    i.type_desc AS [Index Type],
    c.name AS [Indexed Column],
    i.is_unique AS [Is Unique],
    i.is_primary_key AS [Is Primary Key],
    i.is_disabled AS [Is Disabled]
FROM 
    sys.indexes i
JOIN 
    sys.tables t 
    ON i.object_id = t.object_id
JOIN 
    sys.index_columns ic 
    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN 
    sys.columns c 
    ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE 
    i.type > 0 -- Exclude Heap indexes (type = 0)
GO