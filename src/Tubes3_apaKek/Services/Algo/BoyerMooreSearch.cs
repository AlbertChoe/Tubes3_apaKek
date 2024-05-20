
namespace Services.Algo{
    public static class BoyerMooreSearch{
    public static int BMSearch(string pattern, string text){
        int n = text.Length;
        int m = pattern.Length;
        Dictionary<char, int> lastOccurrence = GetLastOccurrenceTable(pattern);

        int i = m - 1; 
        int j = m - 1; 

        while (i < n)
        {
            if (pattern[j] == text[i])
            {
                if (j == 0)
                    return i; 
                i--;
                j--;
            }
            else
            {
                i += m - Math.Min(j, 1 + lastOccurrence.GetValueOrDefault(text[i], -1));
                j = m - 1; 
            }
        }

        return -1; 
     }

    public static List<int> BMSearchAll(string pattern, string text)
    {
        List<int> occurrences = new List<int>();
        int n = text.Length;
        int m = pattern.Length;
        Dictionary<char, int> lastOccurrence = GetLastOccurrenceTable(pattern);

        int i = m - 1;

        while (i < n)
        {
            int j = m - 1; 
            while (j >= 0 && pattern[j] == text[i - (m - 1 - j)])
            {
                j--;
            }
            
            if (j < 0)
            {
                occurrences.Add(i - m + 1); 
                i += m - lastOccurrence.GetValueOrDefault(text[i], -1);
            }
            else
            {
                i += Math.Max(1, j - lastOccurrence.GetValueOrDefault(text[i - (m - 1 - j)], -1));
            }
        }

        return occurrences;
    }


    private static Dictionary<char, int> GetLastOccurrenceTable(string pattern)
    {
        var lastOccurrence = new Dictionary<char, int>();
        for (int i = 0; i < pattern.Length; i++)
        {
            lastOccurrence[pattern[i]] = i;
        }
        return lastOccurrence;
    }
}
}
