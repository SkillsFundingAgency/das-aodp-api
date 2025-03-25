CREATE VIEW [funded].[v_MS_KPI_NQR_TU_003]
AS
/*##################################################################################################
	-Name:				NQR TU 003
	-Description:		AO average time from first saved draft of a form to submitting the form 
						(grouped by form name and version)

						KPI Indicator Type - Take-Up (DfE)
						Baseline - 2 weeks
						Target - < 3 Days

						
	-Date of Creation:	18/03/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		18/03/2025			Original
##################################################################################################*/

select  MM_MSG.ApplicationId
	   ,A.QualificationNumber
	   ,AO.RecognitionNumber
	   ,AO.Id as AwardingOrganisationId
	   ,AO.NameLegal As AwardingOrganisationName
	   ,MM_MSG.Type
	   ,MM_MSG.SentAt


from 

(Select  M.ApplicationId
	   ,M.Type
	   ,Min(M.SentAt) as SentAt
   
From dbo.Messages M 

Where M.Type in ('ApplicationSubmitted', 'Draft')
Group By M.ApplicationId, M.Type ) As MM_MSG

Left Outer Join dbo.Applications A ON A.Id = MM_MSG.ApplicationId
Left Outer Join dbo.AwardingOrganisation AO ON A.OrganisationId = AO.Id