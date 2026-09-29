namespace SFA.DAS.Aodp.Application.Validation
{
    public static class TextValidationRules
    {
        public static bool IsFreeTextValid(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            if (text.Contains('<') || text.Contains('>'))
                return false;

            foreach (char c in text)
            {
                if (char.IsControl(c) && c is not ('\r' or '\n' or '\t'))
                    return false;
            }

            return true;
        }
    }
}
