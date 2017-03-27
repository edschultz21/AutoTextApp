using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace STStressTesting
{
    class ThreadTest
    {
        Properties.Settings _settings;
        object _lock = new object();
        List<long> _timingResults = new List<long>();

        public void Run(Properties.Settings settings)
        {
            _settings = settings;

            StringBuilder sb = new StringBuilder();
            Helpers.WriteHeader(sb);

            var startThread = _settings.startThread;
            var endThread = _settings.endThread;
            var incThread = _settings.incThread;

            try
            {
                for (int i = startThread; i <= endThread; i += incThread)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Thread count: {i}");
                    RunWithNumThreads(sb, i);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            Helpers.WriteFile(@"C:\temp\StressResultsThread.csv", sb);
        }

        private void RunWithNumThreads(StringBuilder sb, int numThreads)
        {
            var numRequests = _settings.endRequests;

            Stopwatch swAll = new Stopwatch();
            swAll.Start();

            try
            {
                var threadList = new List<Thread>();
                for (int i = 0; i < numRequests; i++)
                {
                    Console.WriteLine($"Working on: {i}");

                    while (threadList.Count < numThreads)
                    {
                        Thread newThread = new Thread(new ThreadStart(ThreadCall), 1024);
                        threadList.Add(newThread);
                        newThread.Start();
                    }
                    if (i < numRequests)
                    {
                        var threadsAvailable = 0;
                        while (threadList.Count == numThreads)
                        {
                            Thread.Sleep(2);
                            threadsAvailable = 0;
                            for (int j = numThreads - 1; j >= 0; j--)
                            {
                                if (!threadList[j].IsAlive)
                                {
                                    threadsAvailable++;
                                    threadList.RemoveAt(j);
                                }
                            }
                        }
                    }
                }
                foreach (Thread thread in threadList)
                {
                    thread.Join();
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            swAll.Stop();

            var results = new StressResults { TotalElapsed = swAll.ElapsedMilliseconds, Results = _timingResults };
            Helpers.ProcessResults(sb, 1, "Multi", results, numRequests, numThreads);
        }

        private void ThreadCall()
        {
            var request = Helpers.CreateWebRequest(_settings.Server, _settings.Pages);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            WebResponse response = request.GetResponse();
            Helpers.ReadResults(response, _settings.IsPDF);
            //Console.WriteLine("IsFromCache? {0}", response.IsFromCache);
            response.Close();
            sw.Stop();

            lock (_lock)
            {
                _timingResults.Add(sw.ElapsedMilliseconds);
            }
        }
    }
}
