namespace SFA.DAS.AODP.Models.Qualifications
{
    public static class AgeGroupHelper
    {
        public static string Build(
            bool? pre16,
            bool? sixteenToEighteen,
            bool? eighteenPlus,
            bool? nineteenPlus)
        {
            var groups = new List<string>();

            if (pre16 == true)
            {
                groups.Add("Pre-16");
            }

            if (sixteenToEighteen == true)
            {
                groups.Add("16-18");
            }

            if (eighteenPlus == true)
            {
                groups.Add("18+");
            }

            if (nineteenPlus == true)
            {
                groups.Add("19+");
            }

            return string.Join(", ", groups);
        }
    }
}
