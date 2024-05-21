using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Services.Tools
{
    public class ImageToAsciiConverter
    {
        private static readonly char[] AsciiChars = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@' };

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

        public static string BitmapImageToAscii(BitmapImage bitmapImage)
        {
            Bitmap bitmap = BitmapImageToBitmap(bitmapImage);
            return ImageToAsciiFromUniqueSegment2(bitmap, 64);
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

        public static string ImageToAsciiFromUniqueSegment2(Bitmap image, int segmentWidth = 32)
        {
            double[] grayscaleArray = ImageToGrayscaleArray(image);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < grayscaleArray.Length; i++)
            {
                int binary = grayscaleArray[i] > 128 ? 1 : 0;
                sb.Append(binary);
            }

            int changeCountMax = 0;
            int bestIndex = 0;
            for (int j = 0; j <= sb.Length - segmentWidth; j += 8)
            {
                int changeCount = CountChange(sb.ToString().Substring(j, segmentWidth));
                
                if (changeCount > changeCountMax)
                {
                    changeCountMax = changeCount;
                    bestIndex = j;
                }
            }

            string selectedBinary = sb.ToString().Substring(bestIndex, segmentWidth);

            // Ensure binary string length is a multiple of 8
            int remainder = selectedBinary.Length % 8;
            if (remainder != 0)
            {
                selectedBinary = selectedBinary.PadRight(selectedBinary.Length + (8 - remainder), '0');
            }

            return BinaryStringToAscii(selectedBinary);
        }

        public static int CountChange(string binaryString)
        {
            int change = 0;
            char prev = binaryString[0];
            for (int i = 1; i < binaryString.Length; i++)
            {
                if (binaryString[i] != prev)
                {
                    change++;
                }
                prev = binaryString[i];
            }

            return change;
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