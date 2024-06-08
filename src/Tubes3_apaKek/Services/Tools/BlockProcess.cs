// SEMENTARA JANGAN PAKE INI DULU
// INI MASIH SALAH, PERBANDINGAN YANG DISPEK BUKAN GNI KEKNYA
using Services.Algo;
namespace Services.Tools{
    public static class BlockProcessor
{
    public static string[] DivideIntoBlocks(string asciiText, int blockSize)
    {
        int numBlocks = (int)Math.Ceiling((double)asciiText.Length / blockSize);
        string[] blocks = new string[numBlocks];

        for (int i = 0; i < numBlocks; i++)
        {
            int startIndex = i * blockSize;
            blocks[i] = asciiText.Substring(startIndex, Math.Min(blockSize, asciiText.Length - startIndex));
        }

        return blocks;
    }

    // public static int FindPatternInBlocks(string pattern, string asciiText, int blockSize)
    // {
    //     string[] blocks = DivideIntoBlocks(asciiText, blockSize);
    //     for (int i = 0; i < blocks.Length; i++)
    //     {
    //         if (KMPSearcher.KMPSearch(pattern, blocks[i]) != -1)
    //         {
    //             return i;
    //         }
    //     }
    //     return -1;
    // }
}
}
