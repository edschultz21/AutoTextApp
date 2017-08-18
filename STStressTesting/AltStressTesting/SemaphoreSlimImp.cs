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
        public int Index { get; set; }
        public StatsAggregator Aggregator { get; set; }
        public SemaphoreSlim Semaphore { get; set; }
    }


    class SemaphoreSlimImp
    {
        static int _queueCount = 0;
        static SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        public void Run(Properties.Settings settings)
        {
            Random rand = new Random(381654729);

            //var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

            var threadList = new List<Thread>();
            var threadSettings = new ThreadSettings { Settings = settings };

            for (int i = 0; i < settings.endRequests; i++)
            {
                Console.WriteLine(_semaphore.CurrentCount);
                if (_queueCount <= settings.RenderRequestMaxQueueLength)
                {
                    Interlocked.Increment(ref _queueCount);

                    //Interlocked.Decrement(ref _queueCount);
                    Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                    threadList.Add(newThread);
                    newThread.Start(threadSettings);

                    //Thread.Sleep(rand.Next(1, 100));
                }
                else
                {
                    Console.WriteLine("Busy: Queue full");
                }
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }

            Console.WriteLine($"Final queue count: {_queueCount}, sema: {_semaphore.CurrentCount}");
        }

        private void ThreadCall(object othreadSettings)
        {
            var threadSettings = (ThreadSettings)othreadSettings;

            try
            {
                Console.WriteLine($"Queue count: {_queueCount}, sema: {_semaphore.CurrentCount}");

                if (_semaphore.Wait(threadSettings.Settings.RenderRequestTimeoutSeconds * 1000))
                {
                    Console.WriteLine("Value: Executing");
                    //Interlocked.Increment(ref _queueCount);

                    try
                    {
                        Stopwatch sw = new Stopwatch();

                        var request = Helpers.CreateWebRequest(threadSettings.Settings.Server, threadSettings.Settings.Pages);

                        sw.Start();
                        WebResponse response = request.GetResponse();
                        var isProblem = Convert.ToInt32(response.Headers["Content-Length"]) < 100;
                        if (isProblem)
                        {
                            Console.WriteLine("Problem encountered");
                        }
                        else
                        {
                            Helpers.ReadResults(response, threadSettings.Settings.IsPDF);
                        }
                        response.Close();
                        sw.Stop();

                        Console.WriteLine(sw.ElapsedMilliseconds.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        _semaphore.Release();
                        //Interlocked.Decrement(ref _queueCount);
                    }
                }
                else
                {
                    Console.WriteLine("Busy: Time out");
                }
            }
            finally
            {
                Interlocked.Decrement(ref _queueCount);
            }
        }
    }
}
