using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace STStressTesting
{
    class SemaphoreSlimDir
    {
        public void Run(Properties.Settings settings)
        {
            Random rand = new Random(381654729);

            var stats = new StatsAggregator();
            //var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

            var threadList = new List<Thread>();
            var threadSettings = new ThreadSettings { Settings = settings, Aggregator = stats, Semaphore = new SemaphoreSlim(settings.numConcurrentRequests, settings.numConcurrentRequests) };

            for (int i = 0; i < settings.numThreads; i++)
            {
                threadSettings.Index = i;
                Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                threadList.Add(newThread);
                newThread.Start(threadSettings);

                //Thread.Sleep(rand.Next(1, 100));
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }

            Console.WriteLine(stats);
        }
        
        private void ThreadCall(object othreadSettings)
        {
            var settings = ((ThreadSettings)othreadSettings).Settings;
            var index = ((ThreadSettings)othreadSettings).Index;
            var stats = ((ThreadSettings)othreadSettings).Aggregator;
            var semaphore = ((ThreadSettings)othreadSettings).Semaphore;


            for (int i = 0; i < settings.numRequestsPerThread; i++)
            {


                try
                {
                    semaphore.Wait();
                    var timer = Stopwatch.StartNew();

                    var request = Helpers.CreateWebRequest(settings.Server, settings.Pages);

                    using (var response = request.GetResponse())
                    {

                        var contentLength = Convert.ToInt32(response.Headers["Content-Length"]);
                        if (contentLength < 200)
                        {
                            var result = Helpers.ReadResults(response, false);
                            Console.WriteLine($"{index}: Problem: {result} ({contentLength})");
                            stats.RecordError(result);
                        }
                        else
                        {
                            Helpers.ReadResults(response, settings.IsPDF);
                            Console.WriteLine($"{index}: PDF returned ({contentLength})");
                            stats.RecordSuccess(timer.ElapsedMilliseconds);
                        }
                    }
                }
                catch (WebException e)
                {
                    stats.RecordError("Too Many");
                }
                finally
                {
                    semaphore.Release();
                }
            }            
        }


    }
}
