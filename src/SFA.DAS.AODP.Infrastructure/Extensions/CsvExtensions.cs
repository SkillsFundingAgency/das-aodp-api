using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AODP.Infrastructure.Extensions;

public static class CsvExtensions
{
    public static string ToCsvSafe(this string s) =>
        s.Length > 0 && "=+-@\t\r".IndexOf(s[0]) >= 0 ? "'" + s : s;
}
