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
        public SemaphoreSlim Semaphore { get; set; }
    }

    class SemaphoreSlimImp
    {
        public void Run(Properties.Settings settings)
        {
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            var threadList = new List<Thread>();
            var threadSettings = new ThreadSettings { Settings = settings, Semaphore = semaphore };

            for (int i = 0; i < settings.endRequests; i++)
            {
                Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
                threadList.Add(newThread);
                newThread.Start(threadSettings);
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }
        }

        private void ThreadCall(object othreadSettings)
        {
            var threadSettings = (ThreadSettings)othreadSettings;

            if (threadSettings.Semaphore.Wait(6000))
            {
                Console.WriteLine("Value: Executing");

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
                    threadSettings.Semaphore.Release();
                }
            }
            else
            {
                Console.WriteLine("Value: Skipped");
            }
        }
    }
}
