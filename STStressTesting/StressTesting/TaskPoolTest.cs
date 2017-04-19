//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Net;
//using System.IO;
//using System.Net.Cache;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Configuration;

//namespace STStressTesting
//{
//    class TaskPoolTest
//    {
//        Properties.Settings _settings;

//        public void Run(Properties.Settings settings)
//        {
//            _settings = settings;

//            StringBuilder sb = new StringBuilder();
//            Helpers.WriteHeader(sb);

//            var startRequests = _settings.startRequests;
//            var endRequests = _settings.endRequests;
//            var incRequests = _settings.incRequests;
        
//            //var numRequestsForIterations = _settings.numRequestsForIterations;
//            //var numIterations = _settings.numIterations;

//            StressResults results = null;

//            try
//            {
//                for (int i = startRequests; i <= endRequests; i += incRequests)
//                {
//                    Console.WriteLine($"Num requests: {i}");

//                    results = Single(i);
//                    Helpers.ProcessResults(sb, 1, "Single", results);

//                    Console.WriteLine();

//                    results = Multi(i);
//                    Helpers.ProcessResults(sb, 1, "Multi", results);

//                    Console.WriteLine("===========================");
//                }

//                // This section only used if you want to try the same number of requests over and over again.
//                //for (int i = 1; i <= numIterations; i++)
//                //{
//                //    Console.WriteLine($"Iteration: {i}");
//                //    Console.WriteLine($"Num requests: {numRequestsForIterations}");

//                //    results = Single(numRequestsForIterations);
//                //    Helpers.ProcessResults(sb, i, "Single", results, numRequestsForIterations);

//                //    Console.WriteLine();

//                //    results = Multi(numRequestsForIterations);
//                //    Helpers.ProcessResults(sb, i, "Multi", results, numRequestsForIterations);

//                //    Console.WriteLine("===========================");
//                //}
//            }
//            catch (Exception ex)
//            {
//                sb.AppendLine(ex.Message);
//            }

//            Helpers.WriteFile(@"C:\temp\StressResults.csv", sb);
//        }

//        private StressResults Multi(int numThreads)
//        {
//            Stopwatch swAll = new Stopwatch();
//            swAll.Start();

//            var tasks = new List<Task<long>>();
//            for (int i = 0; i < numThreads; i++)
//            {
//                tasks.Add(Task<long>.Run(async () =>
//                {
//                    var request = Helpers.CreateWebRequest(_settings.Server, _settings.Pages);

//                    Stopwatch sw = new Stopwatch();
//                    sw.Start();
//                    WebResponse response = await request.GetResponseAsync();
//                    Helpers.ReadResults(response, _settings.IsPDF);
//                    //Console.WriteLine("IsFromCache? {0}", response.IsFromCache);
//                    response.Close();
//                    sw.Stop();

//                    Console.WriteLine($"{i}: {sw.ElapsedMilliseconds}");

//                    return sw.ElapsedMilliseconds;
//                }));
//            }
//            Task.WaitAll(tasks.ToArray());

//            swAll.Stop();

//            var results = tasks.Select(x => x.Result).ToList();
//            return new StressResults { TotalElapsed = swAll.ElapsedMilliseconds, Results = results };
//        }

//        private StressResults Single(int numThreads)
//        {
//            Stopwatch swAll = new Stopwatch();
//            swAll.Start();

//            var tasks = new List<long>();
//            for (int i = 0; i < numThreads; i++)
//            {
//                var request = Helpers.CreateWebRequest(_settings.Server, _settings.Pages);

//                Stopwatch sw = new Stopwatch();

//                sw.Start();
//                WebResponse response = request.GetResponse();
//                Helpers.ReadResults(response, _settings.IsPDF);
//                //Console.WriteLine("IsFromCache? {0}", response.IsFromCache);
//                response.Close();
//                sw.Stop();

//                Console.WriteLine($"{i}: {sw.ElapsedMilliseconds}");

//                tasks.Add(sw.ElapsedMilliseconds);
//            }
//            swAll.Stop();

//            return new StressResults { TotalElapsed = swAll.ElapsedMilliseconds, Results = tasks };
//        }
//    }
//}
