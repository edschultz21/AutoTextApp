using System;
using Nest;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Elastic
{
    public class Data
    {
        private ElasticClient _client;

        public Data(ElasticClient client)
        {
            _client = client;
        }

        public void Load()
        {
            // Too lazy to load these for throw away code.
            var segmentDefinitionMap = new Dictionary<string, string>();
            segmentDefinitionMap.Add("1", "countygroup");
            segmentDefinitionMap.Add("3", "city");
            segmentDefinitionMap.Add("4", "zipcode");
            segmentDefinitionMap.Add("6", "school");
            segmentDefinitionMap.Add("7", "county");
            segmentDefinitionMap.Add("8", "organization");
            segmentDefinitionMap.Add("10", "agent");
            segmentDefinitionMap.Add("11", "office");
            segmentDefinitionMap.Add("12", "firm");

            LoadDataFromDB<SegmentDocument>(
                "Data Source=.;Initial Catalog=Infoserv;Integrated Security=true",
                "SELECT count(*) " +
                "FROM [dbo].[Configuration_Segment] b " +
                "JOIN [dbo].[Segment] a ON a.SegmentKey = b.SegmentId",
                "SELECT a.SegmentKey, a.DisplayName, b.SegmentDefinitionKey " +
                "FROM [dbo].[Configuration_Segment] b " +
                "JOIN [dbo].[Segment] a ON a.SegmentKey = b.SegmentId",
                (reader) =>
                {
                    var segmentDefinitionKey = reader["SegmentDefinitionKey"].ToString();
                    var segmentGroupKey = segmentDefinitionMap.ContainsKey(segmentDefinitionKey) ?
                        segmentDefinitionMap[segmentDefinitionKey] : "unknown";

                    return new SegmentDocument
                    {
                        SegmentKey = reader["SegmentKey"].ToString(),
                        SegmentGroup = segmentGroupKey,
                        DisplayName = reader["DisplayName"].ToString()
                    };
                }
            );
        }

        private void WriteDataToSearchEngine<T>(List<SegmentDocument> items) where T : class
        {
            var descriptor = new BulkDescriptor();

            descriptor.IndexMany<SegmentDocument>(items, (x, y) => x.Id(((SegmentDocument)y).SegmentKey));
            items.Clear();
            var response = _client.Bulk(b => descriptor);
        }

        private void LoadDataFromDB<T>(string sqlConnection, string sqlCountCommand, string sqlCommand, Func<SqlDataReader, object> createItem) where T : class
        {
            var BULK_WRITE_AMOUNT = 10000;

            var itemCount = 0;
            var itemTotal = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var memoryStart = GC.GetTotalMemory(true);

            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlCountCommand, connection))
                {
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    itemTotal = count / BULK_WRITE_AMOUNT;
                }

                using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var items = new List<SegmentDocument>();

                        while (reader.Read())
                        {
                            if (items.Count == BULK_WRITE_AMOUNT)
                            {
                                WriteDataToSearchEngine<T>(items);
                            }

                            var document = (SegmentDocument)createItem(reader);

                            items.Add(document);

                            itemCount++;
                            if (itemCount % BULK_WRITE_AMOUNT == 0)
                            {
                                Console.WriteLine($"{itemCount / BULK_WRITE_AMOUNT} of {itemTotal}");
                            }
                        }

                        if (items.Count > 0)
                        {
                            WriteDataToSearchEngine<T>(items);
                        }
                    }
                }
            }

            sw.Stop();
            Console.WriteLine("Elapsed time: " + sw.Elapsed.ToString());
        }
    }
}
