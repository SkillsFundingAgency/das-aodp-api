namespace SFA.DAS.AODP.Models.Validation
{
    public static class QanValidationMessages
    {
        public const string QualificationNotFound =
            @"The QAN number does not match an Ofqual record.";

        public const string TitleAndOrganisationMismatch =
            @"The QAN you entered does not match the qualification title and AO you’ve provided.";

        public const string TitleMismatch =
            @"The title of this qualification in the Ofqual Register does not match the information in the application form.";

        public const string OrganisationMismatch =
            @"This QAN does not match any qualifications associated with your AO.";
    }
}
