CREATE VIEW [dbo].[v_ChangedQualification] as


SELECT 
	[Id],
	[Qan],
	[QualificationName]
FROM [dbo].[Qualification]
WHERE 
	(SELECT COUNT(*) FROM [regulated].[QualificationVersions]) > 1;
