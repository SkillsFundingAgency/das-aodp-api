Create view [dbo].[v_DataDictionary_Permissions] AS

SELECT 
    dp.name AS [Role Name], 
    dp.type_desc AS [Role Type], 
    o.name AS [Object Name], 
    o.type_desc AS [Object Type],
    p.permission_name AS [Permission Name], 
    p.state_desc AS [Permission State]
FROM 
    sys.database_permissions p
JOIN 
    sys.database_principals dp 
    ON p.grantee_principal_id = dp.principal_id
LEFT JOIN 
    sys.objects o 
    ON p.major_id = o.object_id
WHERE 
    dp.type IN ('R', 'S', 'U') -- Roles, SQL users, and User-defined database roles
GO