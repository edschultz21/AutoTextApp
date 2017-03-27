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
            sb.AppendLine("Iteration,Type,Thread Count, Num Requests,Total Elapsed (ms),Total Aggregate (ms),Avg Response Time (ms), Success per Minute");
        }

        public static void WriteFile(string filename, StringBuilder sb)
        {
            File.WriteAllText(filename, sb.ToString());
        }

        public static void ProcessResults(StringBuilder sb, int iteration, string header, StressResults results, int numResults, int threadCount = 0)
        {
            var totalTime = results.Results.Sum();

            Console.WriteLine($"Results for {header} iteration {iteration}");
            Console.WriteLine($"Total elapsed: {results.TotalElapsed}");
            Console.WriteLine($"Total aggregate: {totalTime}");

            // Note that averages are for throughput in real time. Hence we always use total elapsed:
            var averageResponseTime = (double)results.TotalElapsed / numResults;
            Console.WriteLine($"Average response time: {averageResponseTime}");
            long successPerMinute = (numResults * 1000 * 60) / results.TotalElapsed;
            Console.WriteLine($"Successful responses per minute: {successPerMinute}");

            var temp = threadCount == 0 ? "pooled" : threadCount.ToString();
            sb.AppendLine($"{iteration},{header},{temp},{numResults},{results.TotalElapsed},{totalTime},{averageResponseTime},{successPerMinute}");
        }

        public static void ReadResults(WebResponse response, bool isPDF)
        {
            if (isPDF)
            {
                var stream = new MemoryStream();
                response.GetResponseStream().CopyTo(stream);
            }
            else
            {
                StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
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
