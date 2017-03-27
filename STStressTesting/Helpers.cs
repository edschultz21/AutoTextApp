using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace STStressTesting
{
    class Helpers
    {
        public static void WriteHeader(StringBuilder sb)
        {
            sb.AppendLine("Iteration,Type,Thread Count,Num Requests,Problems,Exceptions,Total Elapsed (ms),Total Aggregate (ms),Avg Response Time (ms), Success per Minute");
        }

        public static void WriteFile(string filename, StringBuilder sb)
        {
            File.WriteAllText(filename, sb.ToString());
        }

        public static void ProcessResults(StringBuilder sb, int iteration, string header, StressResults results, int threadCount = 0)
        {
            var totalTime = results.Results.Sum();

            Console.WriteLine();
            Console.WriteLine($"Results for {header} iteration {iteration}");
            Console.WriteLine($"Total elapsed: {results.TotalElapsed}");
            Console.WriteLine($"Total aggregate: {totalTime}");
            Console.WriteLine($"Success: {results.Results.Count}, Problems: {results.TotalProblems}, Exceptions: {results.TotalExceptions}");

            // Note that averages are for throughput in real time. Hence we always use total elapsed:
            var averageResponseTime = (double)results.TotalElapsed / results.TotalRequests;
            Console.WriteLine($"Average response time: {averageResponseTime}");
            long successPerMinute = (results.TotalRequests * 1000 * 60) / results.TotalElapsed;
            Console.WriteLine($"Successful responses per minute: {successPerMinute}");

            var temp = threadCount == 0 ? "pooled" : threadCount.ToString();
            sb.AppendLine($"{iteration},{header},{temp},{results.TotalRequests},{results.TotalProblems},{results.TotalExceptions},{results.TotalElapsed},{totalTime},{averageResponseTime},{successPerMinute}");
        }

        public static void ReadResults(WebResponse response, bool isPDF)
        {
            if (isPDF)
            {
                using (var stream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(stream);
                }
            }
            else
            {
                using (var sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    string result = sr.ReadToEnd();
                }
            }
        }

        public static HttpWebRequest CreateWebRequest(string server, string pages)
        {
            List<string> pagesList = new List<string>(pages.Split(','));

            Random rand = new Random();

            Stopwatch sw = new Stopwatch();

            var next = rand.Next(pagesList.Count);
            var uri = $"http://{server}/infoserv/s-v1/{pagesList[next]}";
            //var uri = $"http://crmls.devstats.showingtime.com/infoserv/s-v1/{pages[next]}";

            var request = WebRequest.CreateHttp(uri);
            request.KeepAlive = false;
            request.Headers.Add("Cache-Control", "no-cache");

            // Define a cache policy for this request only. 
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;

            return request;
        }
    }
}
