using Markdig;

namespace SFA.DAS.AODP.Application;

public static class HTMLGenerator
{
    public static string FromMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return string.Empty;
        return Markdown.ToHtml(markdown)
            .Replace("<a", "<a class=\"govuk-link\" target=\"_blank\"");
    }
}
