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
            sb.AppendLine(
                "Thread Count," +
                "Num Requests," +
                "Problems," +
                "Exceptions," +
                "Total Elapsed (ms)," +
                "Total Aggregate (ms)," +
                "Avg Response Time (ms)," +
                "Responses per Minute," +
                "Overall Throughput per Minute"
            );
        }

        public static void WriteFile(string filename, StringBuilder sb)
        {
            File.WriteAllText(filename, sb.ToString());
        }

        public static void ProcessResults(StringBuilder sb, int iteration, string header, StressResults results, int threadCount = 0)
        {
            var totalAggregate = results.Results.Sum();
            var totalRequestsSuccessful = results.TotalRequests - results.TotalProblems - results.TotalExceptions;
            var averageResponseTime = totalRequestsSuccessful == 0 ? 0.0 : ((double)totalAggregate / totalRequestsSuccessful);
            int responsesPerMinute = averageResponseTime == 0 ? 0 : System.Convert.ToInt32(1000 * 60 / averageResponseTime);
            int overallThroughput = responsesPerMinute * threadCount;

            Console.WriteLine();
            Console.WriteLine($"Results for {header} iteration {iteration}");
            Console.WriteLine($"Total elapsed: {results.TotalElapsed}, Total aggregate: {totalAggregate}");
            Console.WriteLine($"Success: {results.Results.Count}, Problems: {results.TotalProblems}, Exceptions: {results.TotalExceptions}");

            Console.WriteLine($"Average response time: {averageResponseTime}");
            Console.WriteLine($"Responses per minute: {responsesPerMinute}");
            Console.WriteLine($"Overall throughput per minute: {overallThroughput}");

            var sthreadCount = threadCount == 0 ? "pooled" : threadCount.ToString();
            sb.AppendLine(
                $"{sthreadCount}," +
                $"{results.TotalRequests}," +
                $"{results.TotalProblems}," +
                $"{results.TotalExceptions}," +
                $"{results.TotalElapsed}," +
                $"{totalAggregate}," +
                $"{averageResponseTime}," +
                $"{responsesPerMinute}" +
                $"{overallThroughput}"
             );
        }

        public static string ReadResults(WebResponse response, bool isPDF)
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
                    return result;
                }
            }

            return string.Empty;
        }

        public static HttpWebRequest CreateWebRequest(string server, string pages)
        {
            List<string> pagesList = new List<string>(pages.Split(','));

            Random rand = new Random();

            Stopwatch sw = new Stopwatch();

            var next = rand.Next(pagesList.Count);
            var uri = $"http://{server}/infoserv/s-v1/{pagesList[next]}";
            //Console.WriteLine(uri);
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
