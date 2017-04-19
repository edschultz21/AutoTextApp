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
    class ThreadSettings
    {
        public Properties.Settings Settings { get; set; }
        //public SemaphoreSlim Semaphore { get; set; }
        public int ServerBusy;
        public int ClientBusy;
        public int Passed;
        public SemaphoreSlim Semaphore { get; set; }
    }

    class ConcurrentTest
    {
        public void Run(Properties.Settings settings)
        {
            var threadList = new List<Thread>();
            var allThreadSettings = new List<ThreadSettings>();

            for (int i = 0; i < settings.NumThreads; i++)
            {
                Thread newThread = new Thread(new ParameterizedThreadStart(SemaphoreThreadCall));
                threadList.Add(newThread);
                var threadSettings = new ThreadSettings { Settings = settings };
                allThreadSettings.Add(threadSettings);
                newThread.Start(threadSettings);
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }

            Console.WriteLine();

            foreach (var threadSettings in allThreadSettings)
            {
                Console.WriteLine($"Passed: {threadSettings.Passed}, Client Busy: {threadSettings.ClientBusy}, Server Busy: {threadSettings.ServerBusy}");
            }
            Console.WriteLine();
            Console.WriteLine("Total");
            Console.WriteLine($"Passed: {allThreadSettings.Sum(x => x.Passed)}, Client Busy: {allThreadSettings.Sum(x => x.ClientBusy)}, Server Busy: {allThreadSettings.Sum(x => x.ServerBusy)}");
        }

        private void SemaphoreThreadCall(object oThreadSettings)
        {
            var threadSettings = (ThreadSettings)oThreadSettings;
            threadSettings.Semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);

            var threadList = new List<Thread>();

            for (int i = 0; i < threadSettings.Settings.NumConcurrentRequests; i++)
            {
                Thread.Sleep(threadSettings.Settings.WaitTimeBetweenRequest);
                Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                threadList.Add(newThread);
                newThread.Start(threadSettings);
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }
        }

        private void ThreadCall(object oThreadSettings)
        {
            var threadSettings = (ThreadSettings)oThreadSettings;

            if (threadSettings.Semaphore.Wait(30000))
            {
                Stopwatch sw = new Stopwatch();

                try
                {
                    var request = Helpers.CreateWebRequest(threadSettings.Settings.Server, threadSettings.Settings.Pages);

                    sw.Start();
                    WebResponse response = request.GetResponse();
                    var contentLength = Convert.ToInt32(response.Headers["Content-Length"]);
                    if (contentLength < 400)
                    {
                        var result = Helpers.ReadResults(response, false);
                        Console.WriteLine($"Problem: {result} ({contentLength})");
                        Interlocked.Increment(ref threadSettings.ServerBusy);
                    }
                    else
                    {
                        //Helpers.ReadResults(response, settings.IsPDF);
                        Console.WriteLine($"PDF returned ({contentLength})");
                        Interlocked.Increment(ref threadSettings.Passed);
                    }
                    response.Close();
                    sw.Stop();

                    //Console.WriteLine(", time: " + sw.ElapsedMilliseconds.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    threadSettings.Semaphore.Release();
                }
            }
            else
            {
                threadSettings.ClientBusy++;
                Console.WriteLine("Busy: Time out");
            }
        }
    }
}
