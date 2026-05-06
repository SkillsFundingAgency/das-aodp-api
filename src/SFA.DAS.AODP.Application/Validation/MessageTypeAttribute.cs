using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Aodp.Application.Application.Validation
{
    public class MessageTypeAttribute : Aodp.Application.Validation.AllowedValuesAttribute
    {
        public MessageTypeAttribute() : base(
            "UnlockApplication",
            "PutApplicationOnHold",
            "RequestInformationFromAOByQfau",
            "RequestInformationFromAOByOfqaul",
            "RequestInformationFromAOBySkillsEngland",
            "ReplyToInformationRequest",
            "InternalNotes",
            "InternalNotesForQfauFromOfqual",
            "InternalNotesForQfauFromSkillsEngland",
            "InternalNotesForPartners",
            "ApplicationSharedWithOfqual",
            "ApplicationSharedWithSkillsEngland",
            "ApplicationUnsharedWithOfqual",
            "ApplicationUnsharedWithSkillsEngland",
            "ApplicationSubmitted") { }

        protected override bool AllowNull => false;
    }
}
