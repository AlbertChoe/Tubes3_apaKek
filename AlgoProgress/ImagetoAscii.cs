using System.Drawing;
using System.Text;

public class ImageToAsciiConverter
{
    public static string ImageToAscii(string imagePath)
    {
        using (Bitmap image = new Bitmap(imagePath))
        {
            StringBuilder sb = new StringBuilder();
            
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int grayScale = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                    int binary = grayScale > 128 ? 1 : 0;
                    sb.Append(binary);
                }
            }

            // Ensure binary string length is a multiple of 8
            string binaryStr = sb.ToString();
            int remainder = binaryStr.Length % 8;
            if (remainder != 0)
            {
                binaryStr = binaryStr.PadRight(binaryStr.Length + (8 - remainder), '0');
            }

            return BinaryStringToAscii(binaryStr);
        }
    }

    private static string BinaryStringToAscii(string binaryString)
    {
        StringBuilder ascii = new StringBuilder();

        for (int i = 0; i < binaryString.Length; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            ascii.Append((char)Convert.ToInt32(byteString, 2));
        }

        return ascii.ToString();
    }
}
