namespace SFA.DAS.Aodp.Application.Validation
{
    public class UserTypeAttribute : AllowedValuesAttribute
    {
        public UserTypeAttribute() : base(
            "AwardingOrganisation",
            "SkillsEngland",
            "Ofqual",
            "Qfau") { }

        protected override bool AllowNull => false;
    }
}
