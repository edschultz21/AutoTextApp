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
        List<long> _timingResults;
        int _numExceptions = 0;
        int _numProblems = 0;
        Semaphore _pool;

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
                    _pool = new Semaphore(i, i);

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
                _timingResults = new List<long>();
                _numProblems = 0;
                _numExceptions = 0;

                var threadList = new List<Thread>();
                for (int i = 0; i < numRequests; i++)
                {
                    Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                    threadList.Add(newThread);
                    newThread.Start(i);
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

            var results = new StressResults {
                TotalElapsed = swAll.ElapsedMilliseconds,
                Results = _timingResults, 
                TotalRequests = numRequests,
                TotalProblems = _numProblems,
                TotalExceptions = _numExceptions
            };

            Helpers.ProcessResults(sb, 1, "Multi", results, numThreads);
        }

        private void ThreadCall(object i)
        {
            _pool.WaitOne();

            var isException = false;
            var isProblem = false;

            Stopwatch sw = new Stopwatch();

            try
            {
                var request = Helpers.CreateWebRequest(_settings.Server, _settings.Pages);

                sw.Start();
                WebResponse response = request.GetResponse();
                isProblem = System.Convert.ToInt32(response.Headers["Content-Length"]) < 100;
                if (!isProblem)
                {
                    Helpers.ReadResults(response, _settings.IsPDF);
                }
                response.Close();
                sw.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isException = true;
            }

            lock (_lock)
            {
                if (!isException && !isProblem)
                {
                    _timingResults.Add(sw.ElapsedMilliseconds);
                }
                _numExceptions += (isException ? 1 : 0);
                _numProblems += (isProblem ? 1 : 0);

                var completed = _numExceptions + _numProblems + _timingResults.Count;
                if (completed % 10 == 0)
                {
                    Console.WriteLine($"Completed: {completed}");
                }
            }

            _pool.Release();
        }
    }
}
