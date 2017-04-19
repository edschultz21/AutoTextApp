//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Net;
//using System.Configuration;

//namespace STStressTesting
//{
//    class SemaphoreSlimDir
//    {
//        private int _tooBusy;
//        private int _passed;

//        public void Run(Properties.Settings settings)
//        {
//            //var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

//            var threadList = new List<Thread>();
//            var threadSettings = new ThreadSettings { Settings = settings };

//            var outerloop = 1;
//            var endloop = settings.endRequests % 100;
//            if (settings.endRequests > 100)
//            {
//                outerloop = (settings.endRequests - 1) / 100 + 1;
//            }

//            for (int j = 1; j <= outerloop; j++)
//            {
//                var loopCount = (j * 100 > settings.endRequests) ? endloop : 100;

//                Console.WriteLine($"{j} of {outerloop}");

//                for (int i = 0; i < loopCount; i++)
//                {
//                    Thread.Sleep(10);
//                    threadSettings.Index = i;
//                    Thread newThread = new Thread(new ParameterizedThreadStart(ThreadCall));
//                    threadList.Add(newThread);
//                    newThread.Start(threadSettings);

//                    //Thread.Sleep(rand.Next(1, 100));
//                }
//                foreach (Thread thread in threadList)
//                {
//                    thread.Join();
//                }
//            }

//            Console.WriteLine($"Passed: {_passed}, Busy: {_tooBusy}");
//        }

//        private void ThreadCall(object othreadSettings)
//        {
//            var settings = ((ThreadSettings)othreadSettings).Settings;
//            var index = ((ThreadSettings)othreadSettings).Index;

//            //Console.Write($"{index}: ");

//            var request = Helpers.CreateWebRequest(settings.Server, settings.Pages);

//            //Console.WriteLine(DateTime.Now.ToLongTimeString());
//            WebResponse response = request.GetResponse();
//            var contentLength = Convert.ToInt32(response.Headers["Content-Length"]);
//            if (contentLength < 400)
//            {
//                var result = Helpers.ReadResults(response, false);
//                Console.WriteLine($"Problem: {result} ({contentLength})");
//                Interlocked.Increment(ref _tooBusy);
//            }
//            else
//            {
//                //Helpers.ReadResults(response, settings.IsPDF);
//                Console.WriteLine($"PDF returned ({contentLength})");
//                Interlocked.Increment(ref _passed);
//            }
//            response.Close();

//        }
//    }
//}
