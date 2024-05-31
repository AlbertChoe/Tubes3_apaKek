using System.Windows.Media.Imaging;
using Tubes3_apaKek.Models;
using Services;
using Services.Tools;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Media;
using Tubes3_apaKek.DataAccess;
using Services.Algo;
using System.IO;
using System.Diagnostics;
namespace Services
{
    public class Logic
    {
        public static ResultData? Search(string algorithm, BitmapImage image, List<string> allpaths)
        {
            

            // WritePathsToFile(allpaths, "pat.txt");
            ResultData result;

            if (allpaths != null && image != null)
            {
                if (algorithm == "KMP")
                {
                    result = KMPController(image, allpaths);
                }
                else
                {

                    result = BMController(image, allpaths);
                }
                return result;
            }
            return null;
        }


        private static void WritePathsToFile(List<string> paths, string filePath)
        {
            try
            {
                Console.WriteLine($"Successfully wrote {paths.Count} paths to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }

        public static ResultData? KMPController(BitmapImage image, List<string> allpaths)
        {

            string image_ascii = ImageToAsciiConverter.BitmapImageToAscii(image);
            int match_number = -1;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string path in allpaths)
            {
                
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                match_number = KMPSearcher.KMPSearch(image_ascii, asciiText);
                if (match_number != -1)
                {
                    string realname = Database.GetRealNameByPath(path); 
                    Biodata data = Database.GetBiodataByRealName(realname);
                    stopwatch.Stop();
                    if(data != null){
                        return new ResultData(data, "KMP", 100, stopwatch.ElapsedMilliseconds,path);
                    }
                    
                }
            }
            stopwatch.Stop();
            return null;
        }

        public static ResultData? BMController(BitmapImage image, List<string> allpaths)
        {

            string image_ascii = ImageToAsciiConverter.BitmapImageToAscii(image);
            int match_number = -1;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string path in allpaths)
            {
                // Console.WriteLine(File.Exists("../../../../../test/Real/100__M_Left_index_finger.BMP"));
                // string pathP = Directory.GetCurrentDirectory();
                // Console.WriteLine(Directory.GetCurrentDirectory());
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                match_number = BoyerMooreSearch.BMSearch(image_ascii, asciiText);
                if (match_number != -1)
                {
                    string realname = Database.GetRealNameByPath(path);
                    Biodata data = Database.GetBiodataByRealName(realname);
                    stopwatch.Stop();
                    return new ResultData(data, "BM", 100, stopwatch.ElapsedMilliseconds,path);
                }
            }

            stopwatch.Stop();
            return null;
        }

        public static ResultData? LDController(BitmapImage image, List<string> allpaths)
        {

            string image_ascii = ImageToAsciiConverter.BitmapImageToAsciiForLD(image);
            double highestSimilarity = 0;
            Biodata bestMatch = null;
            string bestPath = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string path in allpaths)
            {
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                double similarity = Services.Algo.LevenshteinDistance.CalculateSimilarity(image_ascii, asciiText);

                if (similarity > highestSimilarity)
                {
                    highestSimilarity = similarity;
                    string realname = Database.GetRealNameByPath(path);
                    bestMatch = Database.GetBiodataByRealName(realname);
                    bestPath = path;

                    if (similarity == 1.0)
                        break;
                }
            }

            stopwatch.Stop();

            if (bestMatch != null)
            {
                return new ResultData(bestMatch, "Levenshtein", highestSimilarity * 100, stopwatch.ElapsedMilliseconds, bestPath);
            }

            return null;
        }

    }
}

