using System.Text.RegularExpressions;

namespace Services.Tools{
    public class AlayChecker
{
    public static bool IsAlayMatch(string kataAlay, string kataAsli)
    {
        string pattern = BuildRegexPattern(kataAsli);
            Console.WriteLine(pattern);
        // Cek apakah pattern sesuai dengan pattern regex
        if (Regex.IsMatch(kataAlay, pattern, RegexOptions.IgnoreCase))
        {
            return true;
        }

        return false;  
    }

    private static string BuildRegexPattern(string original)
    {
        var words = original.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var patternBuilder = new System.Text.StringBuilder("^");

        foreach (string word in words)
        {
            AppendSpaceIfNeeded(patternBuilder);
            AppendWordPattern(word, patternBuilder);
        }

        patternBuilder.Append("$");
        return patternBuilder.ToString();
    }

    private static void AppendSpaceIfNeeded(System.Text.StringBuilder patternBuilder)
    {
        if (patternBuilder.Length > 1)
            patternBuilder.Append("\\s*");
    }

    private static void AppendWordPattern(string word, System.Text.StringBuilder patternBuilder)
    {
        foreach (char ch in word)
        {
            string charPattern = GetCharPattern(ch);
            patternBuilder.Append($"({charPattern})?");
        }
    }

    private static string GetCharPattern(char ch)
    {
        if ("aeiouAEIOUgGtTsS".Contains(ch))
        {
            return ch switch
            {
                'a' or 'A' => "[aA4]",
                'e' or 'E' => "[eE3]",
                'i' or 'I' => "[iI1]",
                'o' or 'O' => "[oO0]",
                'u' or 'U' => "[uU]",
                'g' or 'G' => "[gG6]",
                's' or 'S' => "[sS5]",
                't' or 'T' => "[tT7]",
                _ => $"[{char.ToLower(ch)}{char.ToUpper(ch)}]"
            };
        }
        return $"[{char.ToLower(ch)}{char.ToUpper(ch)}]";
    }
}
}
