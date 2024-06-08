
using System.Diagnostics;
using Services.Tools;

namespace Services.Algo{
    public class BoyerMooreSearch{

        private Dictionary<char, int> lastOccurrence;

        private string pattern;

        public BoyerMooreSearch(string pattern){
            lastOccurrence = new Dictionary<char, int>();
            this.pattern = pattern;

            lastOccurrence = GetLastOccurrenceTable(pattern);
        }

    public int BMSearch(string text){
          
        int n = text.Length;
        int m = pattern.Length;

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

    public List<int> BMSearchAll(string text)
    {
        List<int> occurrences = new List<int>();
        int n = text.Length;
        int m = pattern.Length;

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


    private Dictionary<char, int> GetLastOccurrenceTable(string pattern)
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
