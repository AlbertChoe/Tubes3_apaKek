
namespace Services.Algo{
    public class KMPSearcher
{
    private int[] lps;
    private int M;

    private string pattern;

    public KMPSearcher(string pattern){
        this.pattern = pattern;
        M = pattern.Length;
        ComputeLPSArray();
    }
    public int KMPSearch(string text)
    {
        int N = text.Length;

        int j = 0;

        int i = 0;
        int result = -1;
        while (i < N)
        {
            if (pattern[j] == text[i])
            {
                j++;
                i++;
            }

            if (j == M)
            {
                result = i - j;
                Console.Write("Found pattern "
                              + "at index " + (i - j));
                j = lps[j - 1];
            }
            else if (i < N && pattern[j] != text[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }
        return result;
    }

    private void ComputeLPSArray()
    {
        lps = new int[M];
        int len = 0;
        int i = 1;
        lps[0] = 0;

        while (i < M)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    len = lps[len - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }
    }

}
}


