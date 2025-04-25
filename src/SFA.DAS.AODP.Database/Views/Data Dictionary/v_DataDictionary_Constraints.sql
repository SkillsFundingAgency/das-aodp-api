Create view [dbo].[v_DataDictionary_Constraints] AS
SELECT 
    o.name AS [Constraint Name],
    o.type_desc AS [Constraint Type],
    t.name AS [Table Name],
    c.name AS [Column Name]
FROM 
    sys.objects o
LEFT JOIN 
    sys.columns c
    ON o.parent_object_id = c.object_id
LEFT JOIN 
    sys.tables t
    ON o.parent_object_id = t.object_id
WHERE 
    o.type IN ('C', 'D', 'F', 'PK', 'UQ') -- Check, Default, Foreign Key, Primary Key, Unique Constraints
GO