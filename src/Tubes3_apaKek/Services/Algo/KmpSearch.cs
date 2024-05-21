
namespace Services.Algo{
    public static class KMPSearcher
{
    public static int KMPSearch(string pattern, string text)
    {
        int M = pattern.Length;
        int N = text.Length;

        int[] lps = new int[M];
        int j = 0;

        ComputeLPSArray(pattern, M, lps);

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

    private static void ComputeLPSArray(string pattern, int M, int[] lps)
    {
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


