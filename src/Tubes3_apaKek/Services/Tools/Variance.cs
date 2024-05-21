using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
namespace Services.Tools
{
    public class Variance
    {
        private static double CalculateVariance(double[] values)
        {
            double mean = values.Average();
            double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Length;
            return variance;
        }

        private static int FindMostUniqueSegment(double[] grayscaleArray, int segmentWidth)
        {
            double maxVariance = 0;
            int bestStartIndex = 0;
            int totalLength = grayscaleArray.Length;

            for (int start = 0; start <= totalLength - 80; start += 8)
            {
                double[] segment = new ArraySegment<double>(grayscaleArray, start, 80).ToArray();
                double variance = CalculateVariance(segment);

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    bestStartIndex = start;
                }
            }

            return bestStartIndex;
        }
        public static string ImageToAsciiFromUniqueSegment(Bitmap image, int segmentWidth = 32)
        {
            double[] grayscaleArray = ImageToGrayscaleArray(image);
            int bestStartIndex = FindMostUniqueSegment(grayscaleArray, segmentWidth);

            StringBuilder sb = new StringBuilder();
            for (int i = bestStartIndex; i < bestStartIndex + segmentWidth; i++)
            {
                int binary = grayscaleArray[i] > 128 ? 1 : 0;
                sb.Append(binary);
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
        private static double[] ImageToGrayscaleArray(Bitmap image)
        {
            double[] grayscaleArray = new double[image.Width * image.Height];
            int index = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    grayscaleArray[index++] = (pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11);
                }
            }

            return grayscaleArray;
        }
    }

}