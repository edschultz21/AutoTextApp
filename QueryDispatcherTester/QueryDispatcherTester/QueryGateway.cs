using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using TenK.InfoSparks.Common.AnalysisServices;
using TenK.InfoSparks.Common.ConnectionConfiguration;
using TenK.InfoSparks.Common.Contracts.QueryDispatcherContract;

namespace QueryDispatcherTester
{
    public class QueryGateway : IDispatcherExecuteCallbackContract
    {
        private readonly string _serverName;
        private readonly string _serverUri;
        private readonly string _serviceKey;
        private readonly int _serverTimeout;

        struct CubeInfo
        {
            public string ServerName;
            public string CubeDatabase;
        }

        // ServerName -> Channel
        private DispatcherProxy _dispatcherChannel;

        private ConcurrentDictionary<string, DispatcherResponseWaiter> _waitingResponses = new ConcurrentDictionary<string, DispatcherResponseWaiter>();

        public QueryGateway(string serverName, string serverUri, string serviceKey, int serverRetryLimit, int serverTimeout)
        {
            _serverName = serverName;
            _serverUri = serverUri;
            _serviceKey = serviceKey;
            _serverTimeout = serverTimeout;

            InitializeDispatcherChannel(_serverName, _serverUri, serverRetryLimit);
        }

        private void InitializeDispatcherChannel(string serverName, string serverUri, int serverRetryLimit)
        {
            _dispatcherChannel = new DispatcherProxy(_serviceKey, serverName, serverUri, serverRetryLimit, this);
        }

        public MDXQueryResult DispatcherRunQuery(string dataSource, string cubeDatabase, string query, int priority = -1)
        {
            var timer = Stopwatch.StartNew();

            string key = Guid.NewGuid().ToString();
            QueryRequest request = new QueryRequest()
            {
                Source = dataSource,
                Database = cubeDatabase,
                ID = key,
                Options = new QueryRequestOptions()
                {
                    Priority = priority
                },
                QueryData = new PartitionQuery()
                {
                    Query = query,
                    Partitions = new PartitionQueryPart[1]
                    {
                        new PartitionQueryPart()
                        {
                            PartitionID = "1",
                            PartitionExpression = ""
                        }
                    }
                }
            };

            MDXQueryResult result;
            DispatcherResponseWaiter waiter = new DispatcherResponseWaiter(_serverTimeout);
            try
            {
                if (!_waitingResponses.TryAdd(key, waiter))
                {
                    Console.WriteLine("Collision Error: QueryKey=" + key + " is already waiting for response.");
                    throw new Exception("Could not handle request at the moment. Please try again.");
                }

                //Console.WriteLine("Running MDX Query on Dispatcher: Server={0}, QueryKey={1}, QueryString={2}", cubeInfo.ServerName, key, query);

                // Perform service call
                _dispatcherChannel.Execute(d => d.ExecuteQuery(_serviceKey, request));

                result = waiter.WaitForAndGetResponse();
            }
            catch (Exception)
            {
                DispatcherResponseWaiter w;
                _waitingResponses.TryRemove(key, out w);
                throw;
            }
            finally
            {
                waiter.Dispose();
            }

            //Console.WriteLine("Dispatcher ran QueryKey={0} on {1} and took {2}ms", key, dataSource, timer.ElapsedMilliseconds);

            return result;
        }

        public void ExecuteQueryComplete(string serviceKey, QueryResponse response)
        {
            //Console.WriteLine("Infoserv received response from Dispatcher. DataSource={0}, QueryKey={1}", response.Source, response.QueryID);
            DispatcherResponseWaiter waiter;
            if (!_waitingResponses.TryRemove(response.QueryID, out waiter))
            {
                Console.WriteLine("Infoserv WARNING: QueryKey={0} not found in waiting keys", response.QueryID);
            }
            else if (response.Status == QueryResponse.ResponseStatusCode.QUERY_ERROR)
            {
                string errorMessage = string.Format("ERROR Dispatcher Response: DataSource={0}, QueryKey={1}", response.Source, response.QueryID);
                Console.WriteLine(errorMessage);
                waiter.SetAndSignalError(errorMessage);
            }
            else
            {
                if (response.Results.Length == 1)
                {
                    waiter.SetResultAndSignalFinished(response.Results[0].Result);
                }
                else
                {
                    // Not supporting until we get to multiparting this. Reasoning being that we could go the partition route
                    // or send multiple different queries. These approaches may or may not take different steps.
                    string errorMessage = "Infoserv ERROR with QueryKey =" + response.QueryID + ": Partioned queries are not supported at this time.";
                    Console.WriteLine(errorMessage);
                    waiter.SetAndSignalError(errorMessage);
                }
            }
        }
    }
}