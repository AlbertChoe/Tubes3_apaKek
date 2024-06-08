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
using System.Data;
namespace Services
{
    public class Logic
    {
        public static ResultData? Search(string algorithm, BitmapImage image, List<string> allpaths)
        {
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
            KMPSearcher kmp = new (image_ascii);
            object lockObj = new();
            ResultData? result = null;


            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
            };

            Parallel.ForEach(allpaths, parallelOptions, (path, state) =>
            {
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                int match_number = kmp.KMPSearch(asciiText);

                if (match_number != -1)
                {
                    lock (lockObj)
                    {
                        if (result == null)
                        {
                            string realname = Database.GetRealNameByPath(path);
                            Biodata data = Database.GetBiodataByRealName(realname);

                            stopwatch.Stop();
                            result = new ResultData(data, "KMP", 100, stopwatch.ElapsedMilliseconds, path);
                            state.Stop();  // Stop the parallel loop
                        }
                    }
                }
            });

            stopwatch.Stop();
            return result;
        }

        public static ResultData? BMController(BitmapImage image, List<string> allpaths)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string image_ascii = ImageToAsciiConverter.BitmapImageToAscii(image);
            BoyerMooreSearch bm = new (image_ascii);
            object lockObj = new();
            ResultData? result = null;


            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
            };

            Parallel.ForEach(allpaths, parallelOptions, (path, state) =>
            {
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                int match_number = bm.BMSearch(asciiText);

                if (match_number != -1)
                {
                    lock (lockObj)
                    {
                        if (result == null)
                        {
                            string realname = Database.GetRealNameByPath(path);
                            Biodata data = Database.GetBiodataByRealName(realname);

                            stopwatch.Stop();
                            result = new ResultData(data, "BM", 100, stopwatch.ElapsedMilliseconds, path);

                            state.Stop();  // Stop the parallel loop
                        }
                    }
                }
            });

            stopwatch.Stop();
            return result;
        }

        public static ResultData? LDController(BitmapImage image, List<string> allpaths)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string image_ascii = ImageToAsciiConverter.BitmapImageToAsciiForLD(image);
            double highestSimilarity = 0;
            Biodata? bestMatch = null;
            string? bestPath = null;
            

            object lockObj = new object();
            CancellationTokenSource cts = new(); 

            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = cts.Token,
            };

            Parallel.ForEach(allpaths, parallelOptions, (path,state) =>
            {
                string asciiText = ImageToAsciiConverter.ImageToAscii(path);
                double similarity = LevenshteinDistance.CalculateSimilarity(image_ascii, asciiText);

                lock (lockObj)
                {
                    if (similarity > highestSimilarity)
                    {
                        highestSimilarity = similarity;
                        string realname = Database.GetRealNameByPath(path);
                        bestMatch = Database.GetBiodataByRealName(realname);
                        bestPath = path;

                        if (similarity == 1.0)
                        {
                            cts.Cancel();
                            state.Stop();
                        }
                    }
                }
            });


            if (bestMatch != null && highestSimilarity * 100 > 50)
            {
                stopwatch.Stop();

                return new ResultData(bestMatch, "Levenshtein", highestSimilarity * 100, stopwatch.ElapsedMilliseconds, bestPath);
            }
            
            stopwatch.Stop();

            return null;
        }

    }
}

