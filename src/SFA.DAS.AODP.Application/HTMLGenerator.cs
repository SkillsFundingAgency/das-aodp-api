using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application;

public static class HTMLGenerator
{
    public static string FromMarkdown(string markdown)
    {
        return Markdown.ToHtml(markdown)
            .Replace("<a", "<a class=\"govuk-link\"");
    }
}
