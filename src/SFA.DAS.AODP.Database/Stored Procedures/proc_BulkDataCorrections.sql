Create PROCEDURE [dbo].[proc_BulkDataCorrections] (@fileURL NVARCHAR(MAX), @WriteCommentFlag int, @SASToken NVARCHAR(MAX), @LocalDeployment int)

/*##################################################################################################
	-Name:				proc_BulkDataCorrections
	-Description:		Bulk Data Corrections via CSV File
						
	-Date of Creation:	26/03/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		26/03/2025			Original
##################################################################################################*/

AS
BEGIN

CREATE TABLE BulkInsertStaging (
	QAN NVARCHAR(MAX)
	,LifecycleStage NVARCHAR(MAX)
	,OutcomeDecision NVARCHAR(MAX)
	,Notes NVARCHAR(MAX)
	,TIMESTAMP NVARCHAR(MAX)
	)
CREATE TABLE #Cleaned (
	QualificationId uniqueidentifier
	,QAN VARCHAR(255)
	,LifecycleStageId uniqueidentifier
	,ProcessStatusId uniqueidentifier
	,Notes NVARCHAR(MAX)
	,TIMESTAMP DATETIME
	)
CREATE TABLE #MissingQualifications (
	QualificationId uniqueidentifier
	,LifecycleStageId uniqueidentifier
	,ProcessStatusId uniqueidentifier
	,TIMESTAMP DATETIME
	)
CREATE TABLE #MergeLog (
    Action NVARCHAR(50),
    QualificationId uniqueidentifier,
    OldLifecycleStageId uniqueidentifier,
    NewLifecycleStageId uniqueidentifier,
	OldProcessStatusId uniqueidentifier,
    NewProcessStatusId uniqueidentifier,
	Notes NVARCHAR(MAX)
)

/*Bulk import data FROM specified CSV and populate staging table
Can be run locally or in cloud using LocalDeployment flag*/
DECLARE @Bulk_SQL VARCHAR(MAX)

If @LocalDeployment = 1 
	Begin
	SET @Bulk_SQL = 'BULK INSERT BulkInsertStaging
	FROM ''' + @fileURL + '''
	WITH (
		FIELDTERMINATOR = '','',  -- Delimiter for columns
		ROWTERMINATOR = ''\n'',   -- Delimiter for rows
		FIRSTROW = 2 )'
	End
ELSE 
	Begin

	/*Create credentials and access to blob
	Create a database scoped credential*/
	DECLARE @SASSQL VARCHAR(MAX)
	SET @SASSQL = 'ALTER DATABASE SCOPED CREDENTIAL maintenance_cred
	WITH IDENTITY = ''SHARED ACCESS SIGNATURE'',
	SECRET = ''' + @SASToken + ''''

	SET @Bulk_SQL = 'BULK INSERT BulkInsertStaging
	FROM ''' + @fileURL +'''
	WITH (DATA_SOURCE = ''ExternalCSVDataSource'', FORMAT = ''CSV'')'


	End 

Exec(@SASSQL) --Create database scoped credential
Exec(@Bulk_SQL) --Execute above SQL statement to populate temp table BulkInsertStaging with CSV data

/*Clean data by forcing into correct format (mainly used for dates) 
and Inserting into temporary cleaned table*/
INSERT INTO #Cleaned (QualificationId, QAN, LifecycleStageId,ProcessStatusId,Notes, Timestamp)
SELECT 
	(SELECT q.Id FROM dbo.Qualification Q Where Cast(stg.QAN as nvarchar(max)) = Q.Qan) as QualificationId,
    stg.QAN, 
    (SELECT L.Id FROM regulated.LifecycleStage L Where Cast(trim(stg.LifecycleStage) as nvarchar(max)) = L.Name) as LifecycleStageId,
	(SELECT P.Id FROM regulated.ProcessStatus P Where Cast(trim(stg.OutcomeDecision) as nvarchar(max)) = P.Name) as ProcessStatusId,
	stg.Notes AS Notes,
    TRY_CONVERT(DATETIME, Timestamp,103) AS Timestamp
FROM BulkInsertStaging stg; 

/*Create list of Qualfications returning only latest version 
and joining in the qualfication id for easy identification*/
WITH TargetMAXVersion AS (
    SELECT   Distinct 
		 QV.QualificationId
		,Q.Qan
		,MAX(QV.Version) Over (Partition By QV.QualificationId) as MAXVersionNo
	FROM regulated.QualificationVersions QV
	Inner Join dbo.Qualification Q	ON Q.Id = QV.QualificationId
)

/*Take the data FROM the imported CSV (via staged and cleaned) and merge with
exisiting Qualification versions table updating existing record statuses*/
MERGE regulated.QualificationVersions AS Target
USING #Cleaned AS Source
ON Target.QualificationId = Source.QualificationId

/*When Qualification already exists update status*/
WHEN MATCHED AND Target.Version = (SELECT MAXVersionNo 
                                      FROM TargetMAXVersion 
                                      WHERE TargetMAXVersion.QualificationId = Source.QualificationId) THEN
    UPDATE SET 
        Target.LifecycleStageId = Source.LifecycleStageId,
        Target.ProcessStatusId = Source.ProcessStatusId

Output  $action AS Operation
	   ,Source.QualificationId
	   ,Deleted.LifecycleStageId
	   ,Inserted.LifecycleStageId
	   ,Deleted.ProcessStatusId
	   ,Inserted.ProcessStatusId
	   ,Source.Notes
	   INTO #MergeLog;

If @WriteCommentFlag = 1 
Begin
/*Write Discussion History records for each updated record in Qualification Version*/
Insert Into dbo.QualificationDiscussionHistory(Id, QualificationId, ActionTypeId, UserDisplayName, Notes, Timestamp, Title)
select	NewID()
		,ML.QualificationId
		,(select Id From dbo.ActionType Where Description = 'Ignore') As ActionTypeId
		,'Support - Bulk Update' AS UserDisplayName
		,ML.Notes AS Notes
		,GetDate() AS Timestamp
		,null AS Title
FROM #MergeLog ML
End

/*Create list of Qualifications in the CSV but not in the Qualification Versions table*/
INSERT INTO #MissingQualifications
SELECT C.QualificationId, C.LifecycleStageId, C.ProcessStatusId, C.TIMESTAMP FROM #Cleaned C
Left Outer Join regulated.QualificationVersions QV ON QV.QualificationId = C.QualificationId
Where QV.QualificationId is null

SELECT * FROM dbo.BulkInsertStaging
SELECT * FROM #Cleaned
SELECT * FROM #MissingQualifications
SELECT * FROM #MergeLog

/*Drop temporary tables*/
DROP TABLE BulkInsertStaging;
DROP TABLE #Cleaned;
DROP TABLE #MissingQualifications;
DROP TABLE #MergeLog;

END
GO