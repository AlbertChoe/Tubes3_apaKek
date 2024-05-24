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
        public static ResultData? Search(string algorithm, BitmapImage image)
        {
            List<string> allpaths = Database.GetAllFingerprintPaths();


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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string image_ascii = ImageToAsciiConverter.BitmapImageToAscii(image);
            int match_number = -1;
            foreach (string path in allpaths)
            {
                
                string ascii_text = ImageToAsciiConverter.ImageToAscii(path);
                match_number = KMPSearcher.KMPSearch(image_ascii, ascii_text);
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string image_ascii = ImageToAsciiConverter.BitmapImageToAscii(image);
            int match_number = -1;
            foreach (string path in allpaths)
            {
                string ascii_text = ImageToAsciiConverter.ImageToAscii(path);
                match_number = BoyerMooreSearch.BMSearch(image_ascii, ascii_text);
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
    }
}