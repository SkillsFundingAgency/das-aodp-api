Create view [dbo].[v_AllAwardingOrganisations] as 

/*##################################################################################################
	-Name:				All Awarding Organisations
	-Description:		View for showing all awarding organisations
						
	-Date of Creation:	17/04/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		17/04/2025			Original
##################################################################################################*/

SELECT  NameLegal
		,NameOfqual
		,Acronym
		,Ukprn 
From AwardingOrganisation 

GO