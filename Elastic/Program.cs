using System;
using System.Data.SqlClient;
using Elasticsearch.Net;
using Nest;

namespace Elastic
{
    class Program
    {
        static void Main(string[] args)
        {
            new MainApp().Run();
        }
    }

    public class MainApp
    {
        private ElasticSearchSegmentStore _store;
        private const string CLIENT_INDEX = "infoserv";

        public void Run()
        {
            Init();

            _store.Clear();
            _store.CreateIndexIfNeeded();

            var data = new Data(_store.Client);
            data.Load();
        }

        public void Init()
        {
            var settings = new ElasticsearchSettings
            {
                Node = "http://localhost:9200/",
                ClientIndex = CLIENT_INDEX,
                MinNgram = 1,
                MaxNgram = 20
            };

            _store = new ElasticSearchSegmentStore(settings);
        }

    }
}
