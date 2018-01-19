using System;
using System.Collections.Generic;
using System.Threading;

namespace QueryDispatcherTester
{
    public struct CubeSource
    {
        public string DataSource;
        public string CubeDatabase;
        public string CubeName;
    }

    public class Program
    {
        public static string SERVER_NAME = "localhost";
        public static string SERVER_URI = "net.tcp://localhost:11081/QueryDispatcher";
        public static string SERVICE_KEY = "QDT";
        public static int SERVER_RETRY_LIMIT = 2;
        public static int SERVER_TIMEOUT = 60000;

        public static string CUBE_NAME_PLACEHOLDER = "%%CUBE_NAME%%";
        public static string NOTHING_QUERY = "SELECT {} ON 0 FROM [" + CUBE_NAME_PLACEHOLDER + "]";
        public static string CITIES_QUERY = "WITH SET [ddd] AS GENERATE([Date].[Year].Members, ORDER([City].[City].Members, [Date].[Year].CURRENTMEMBER, BDESC)) SELECT HEAD([ddd], 10) ON 0 FROM [" + CUBE_NAME_PLACEHOLDER + "]";

        private static List<CubeSource> _dataSources;
        private static QueryGateway _gateway;


        public static void Main(string[] args)
        {
            Init();

            Test();
        }

        private static void Init()
        {
            _dataSources = new List<CubeSource>();
            _dataSources.Add(new CubeSource()
            {
                DataSource = "CRMLS",
                CubeDatabase = "CRMLS_MVB",
                CubeName = "CRMLS"
            });
            _dataSources.Add(new CubeSource()
            {
                DataSource = "MIRealSource_MVMLS",
                CubeDatabase = "MIRealSource_MVMLS",
                CubeName = "MIRealSource_MVMLS"
            });

            _gateway = new QueryGateway(SERVER_NAME, SERVER_URI, SERVICE_KEY, SERVER_RETRY_LIMIT, SERVER_TIMEOUT);
        }

        private static Thread CreateQueryThread(string key, CubeSource source, string query, int priority)
        {
            return new Thread(() =>
                {
                    Console.WriteLine($"Thread {key} started on {source.DataSource} with priority {priority}.");
                    _gateway.DispatcherRunQuery(
                        source.DataSource,
                        source.CubeDatabase,
                        query.Replace(CUBE_NAME_PLACEHOLDER, source.CubeName),
                        priority
                    );
                    Console.WriteLine($"Thread {key} finished on {source.DataSource} with priority {priority}.");
                }
            );
        }

        private static int PickSource(int thread, int max, int numThreads)
        {
            return thread / (numThreads / max);
            //return thread % max;
        }

        private static void Test()
        {
            int numThreads = 20;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                string key = i.ToString();
                int priority = 10 - i;
                CubeSource source = _dataSources[PickSource(i, _dataSources.Count, numThreads)];
                threads[i] = CreateQueryThread(key, source, CITIES_QUERY, priority);
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
                Thread.Sleep(100);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
}
