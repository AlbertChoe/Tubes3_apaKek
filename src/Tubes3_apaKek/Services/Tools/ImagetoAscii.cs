using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Services.Tools
{
    public class ImageToAsciiConverter
    {
        private static readonly char[] AsciiChars = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@' };

        public static string ImageToAscii(string imagePath)
        {

            // MessageBox.Show(imagePath);
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
            return ImageToAsciiFromUniqueSegment(bitmap, 32);
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

        private static int FindMostUniqueSegment(double[] grayscaleArray, int segmentWidth)
        {
            double maxVariance = 0;
            int bestStartIndex = 0;
            int totalLength = grayscaleArray.Length;

            for (int start = 0; start <= totalLength - segmentWidth; start += segmentWidth)
            {
                double[] segment = new ArraySegment<double>(grayscaleArray, start, segmentWidth).ToArray();
                double variance = CalculateVariance(segment);

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    bestStartIndex = start;
                }
            }

            return bestStartIndex;
        }



        private static double CalculateVariance(double[] values)
        {
            double mean = values.Average();
            double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Length;
            return variance;
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


    }
}