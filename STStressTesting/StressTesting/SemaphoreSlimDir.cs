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

            //var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

            var threadList = new List<Thread>();
            var threadSettings = new ThreadSettings { Settings = settings };

            for (int i = 0; i < settings.endRequests; i++)
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
        }
        
        private void ThreadCall(object othreadSettings)
        {
            var settings = ((ThreadSettings)othreadSettings).Settings;
            var index = ((ThreadSettings)othreadSettings).Index;

            //Console.Write($"{index}: ");

            var request = Helpers.CreateWebRequest(settings.Server, settings.Pages);

            WebResponse response = request.GetResponse();
            var contentLength = Convert.ToInt32(response.Headers["Content-Length"]);
            if (contentLength < 200)
            {
                var result = Helpers.ReadResults(response, false);
                Console.WriteLine($"Problem: {result} ({contentLength})");
            }
            else
            {
                Helpers.ReadResults(response, settings.IsPDF);
                Console.WriteLine($"PDF returned ({contentLength})");
            }
            response.Close();
        }
    }
}
