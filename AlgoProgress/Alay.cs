
// YANG NANTI KITA PAKE MO YG RETURN KEK GNII APA BANDINGIN TERUS RETURN TRUE FALSE???
// INI UBAH DAN RETURN NON ALAY
// MASIH PERLU DI UBAH UBAH INI HARUSNYA
public static class AlayTextConverter
{
    // Regex untuk mengganti angka dengan huruf yang sesuai
    private static readonly Regex NumberToLetterRegex = new Regex("[a4@]|[e3]|[i1!]|[o0]|[s5]|[t7]|[g6]", RegexOptions.IgnoreCase);

    // Regex untuk menangani kombinasi huruf besar dan kecil
    private static readonly Regex MixedCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.IgnoreCase);

    // Regex untuk menangani singkatan
    private static readonly Regex AbbreviationRegex = new Regex(@"\bbntng\b|\bdw\b|\bmrthn\b|\bind\b|\bindon\b", RegexOptions.IgnoreCase);

    public static string ConvertAlayToNormal(string alayText)
    {
        // Langkah 1: Mengganti angka dengan huruf yang sesuai
        alayText = NumberToLetterRegex.Replace(alayText, m =>
        {
            return m.Value.ToLower() switch
            {
                "4" => "a",
                "@" => "a",
                "3" => "e",
                "1" => "i",
                "!" => "i",
                "0" => "o",
                "5" => "s",
                "7" => "t",
                "6" => "g",
                _ => m.Value
            };
        });

        // Langkah 2: Menangani kombinasi huruf besar dan kecil
        alayText = alayText.ToLower(); // Menyederhanakan dengan mengubah semua huruf menjadi huruf kecil

        // Langkah 3: Menangani singkatan
        alayText = AbbreviationRegex.Replace(alayText, m =>
        {
            return m.Value switch
            {
                "bntng" => "bintang",
                "dw" => "dwi",
                "mrthn" => "marthen",
                "ind" => "indonesia",
                "indon" => "indonesia",
                _ => m.Value
            };
        });

        return alayText;
    }
}
