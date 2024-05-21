using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using Services.Algo;
namespace Services.Tools
{
    public class EntropyProccess
    {
        // JIKA INGIN PAKAI FUNGSI ENTROPY INI GANTI DI 
        // Logic KMPController ganti image_ascii jadi pake EntropyProccess.FindMostUniqueBinarySegmentUsingEntropy
        // Logic BMController ganti image_ascii jadi pake EntropyProccess.FindMostUniqueBinarySegmentUsingEntropy
        private static int FindMostUniqueBinarySegmentUsingEntropy(string binaryString, int segmentWidth)
        {
            double maxEntropy = 0;
            int bestStartIndex = 0;
            int totalLength = binaryString.Length;

            for (int start = 0; start <= totalLength - segmentWidth; start += 8)  // Skip by 8 bits for each new segment
            {
                double[] segmentValues = new double[segmentWidth];

                for (int i = 0; i < segmentWidth; i++)
                {
                    segmentValues[i] = binaryString[start + i] - '0'; // Convert char '0'/'1' to int 0/1
                }

                double entropy = CalculateEntropy(segmentValues);

                if (entropy > maxEntropy)
                {
                    maxEntropy = entropy;
                    bestStartIndex = start;
                }
            }

            return bestStartIndex;
        }
        private static string ImageToBinaryString(Bitmap image)
        {
            StringBuilder binaryString = new StringBuilder(image.Width * image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    double grayScale = (pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11);
                    int binary = grayScale > 128 ? 1 : 0;
                    binaryString.Append(binary);
                }
            }

            return binaryString.ToString();
        }

        

        private static double CalculateEntropy(double[] values)
        {
            Dictionary<double, int> histogram = new Dictionary<double, int>();
            foreach (double value in values)
            {
                if (histogram.ContainsKey(value))
                    histogram[value]++;
                else
                    histogram[value] = 1;
            }

            double entropy = 0.0;
            int total = values.Length;
            foreach (var item in histogram)
            {
                double probability = (double)item.Value / total;
                entropy -= probability * Math.Log(probability, 2); // using base 2 for information entropy
            }

            return entropy;
        }

        public static string ImageToAsciiFromUniqueSegmentUsingEntropy(BitmapImage bitmapImage, int segmentWidth = 80)
        {
            Bitmap bitmap = BitmapImageToBitmap(bitmapImage);  // Convert BitmapImage to Bitmap
            string binaryString = ImageToBinaryString(bitmap);  // Convert image to binary string
            int bestStartIndex = FindMostUniqueBinarySegmentUsingEntropy(binaryString, segmentWidth);  // Find unique segment

            // Extract the most unique binary segment
            string bestSegment = binaryString.Substring(bestStartIndex, segmentWidth);

            // Ensure binary string length is a multiple of 8
            int remainder = bestSegment.Length % 8;
            if (remainder != 0)
            {
                bestSegment = bestSegment.PadRight(bestSegment.Length + (8 - remainder), '0');
            }

            return BinaryStringToAscii(bestSegment);  // Convert binary to ASCII
        }
        private static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
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
}