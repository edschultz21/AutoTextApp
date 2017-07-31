using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;

namespace Elastic
{
    public class ElasticSearchSegmentStore : ISegmentSearchProvider
    {
        public ElasticClient Client { get; private set; }
        private ElasticsearchSettings _settings;

        private const string ELASTICSEARCH_DOC_TYPE = "segmentdocument"; // MUST be lower case!
        private const string DEFAULT_TOKENIZER = "standard";
        private const string SYNONYM_FILTER = "synonym_filter";
        private const string AUTOCOMPLETE_FILTER = "autocomplete_filter";
        private const string AUTOCOMPLETE_ANALYZER = "autocomplete_analyzer";

        public ElasticSearchSegmentStore(ElasticsearchSettings settings)
        {
            _settings = settings;

            // Sanity checks
            if (string.IsNullOrEmpty(settings.Node))
            {
                throw new ArgumentException("Node not specified for Elasticsearch");
            }
            if (string.IsNullOrEmpty(settings.ClientIndex))
            {
                throw new ArgumentException("No index specified for Elasticsearch");
            }

            // Dictates how much of the start of a word MUST match.
            settings.FuzzyPrefixLength = (settings.FuzzyPrefixLength < 0) ? 2 : settings.FuzzyPrefixLength;

            // Number of shards should ALWAYS be 1. No need for more and it causes problems.
            settings.NumberOfShards = (settings.NumberOfShards <= 0) ? 1 : settings.NumberOfShards;
            settings.NumberOfReplicas = (settings.NumberOfReplicas <= 0) ? 1 : settings.NumberOfReplicas;

            settings.MinNgram = (settings.MinNgram <= 0) ? 1 : ((settings.MinNgram > 20) ? 20 : settings.MinNgram);
            settings.MaxNgram = (settings.MaxNgram <= 0) ? 1 : ((settings.MaxNgram > 20) ? 20 : settings.MaxNgram);

            // Create the client
            Client = CreateClient();
        }

        #region Setup

        private ElasticClient CreateClient()
        {
            var nodes = new Uri[] { new Uri(_settings.Node) };

            var connectionPool = new StaticConnectionPool(nodes);
            var connectionSettings = new ConnectionSettings(connectionPool);
            if (!String.IsNullOrEmpty(_settings.ClientIndex))
            {
                connectionSettings.DefaultIndex(_settings.ClientIndex);
            }

            return new ElasticClient(connectionSettings);
        }

        public void CreateIndexIfNeeded()
        {
            if (!string.IsNullOrEmpty(_settings.ClientIndex))
            {
                if (!Client.IndexExists(_settings.ClientIndex).Exists)
                {
                    // Client index does not exist. Must build one.

                    // Create settings first.
                    var settingsDescriptor = CreateIndexSettingsDescriptor();

                    // Add auto complete settings.
                    AddAutoCompleteSettings(settingsDescriptor);

                    // Get a default descriptor for this index.
                    var indexDescriptor = CreateIndexDescriptor(settingsDescriptor);

                    // Add any mappings to the descriptor.
                    AddMappings(indexDescriptor);

                    CreateIndex(indexDescriptor);
                }
            }
        }

        private IndexSettingsDescriptor CreateIndexSettingsDescriptor()
        {
            var descriptor = new IndexSettingsDescriptor();
            descriptor.NumberOfShards(_settings.NumberOfShards);
            descriptor.NumberOfReplicas(_settings.NumberOfReplicas);

            return descriptor;
        }

        private SynonymTokenFilterDescriptor CreateSynonymDescriptor()
        {
            // Note that because we are doing ngram, we MUST make an alias to ourselves. Otherwise
            // we will not match in simple cases.
            if (!string.IsNullOrEmpty(_settings.SynonymPath))
            {
                var synonymDescriptor = new SynonymTokenFilterDescriptor();

                synonymDescriptor.SynonymsPath(_settings.SynonymPath);
                synonymDescriptor.IgnoreCase(true);
                synonymDescriptor.Tokenizer(DEFAULT_TOKENIZER);

                return synonymDescriptor;
            }

            return null;
        }

        private void AddAutoCompleteSettings(IndexSettingsDescriptor descriptor)
        {
            var synonymDescriptor = CreateSynonymDescriptor();

            // NOTE:
            //      autocomplete_analyzer - name of our custom analyzer. Will be used later to specify
            //          that we want to use this analyzer on the query.
            //      autocomplete_filter - specifies the filter to use for the tokenizer.
            //          The name in the filters list MUST match that for the NGram call.
            // Also, order below is important.
            //
            // A comment on searching by case:
            //      CASE SENSITIVE - Remove "lowercase" below and DO NOT modify query string from uers.
            //      CASE INSENSITIVE - Leave "lowercase" below and modify query from user to be lower case.
            if (synonymDescriptor != null)
            {
                descriptor.Analysis(a => a
                    .TokenFilters(f => f
                        .Synonym(SYNONYM_FILTER, s => synonymDescriptor)
                        .EdgeNGram(AUTOCOMPLETE_FILTER, g => g.MinGram(_settings.MinNgram).MaxGram(_settings.MaxNgram)))
                    .Analyzers(an => an
                        .Custom(AUTOCOMPLETE_ANALYZER, c => c
                            .Tokenizer(DEFAULT_TOKENIZER)
                            .Filters(new List<string> { "lowercase", AUTOCOMPLETE_FILTER, SYNONYM_FILTER }))));
            }
            else
            {
                descriptor.Analysis(a => a
                    .TokenFilters(f => f
                        .EdgeNGram(AUTOCOMPLETE_FILTER, g => g.MinGram(_settings.MinNgram).MaxGram(_settings.MaxNgram)))
                    .Analyzers(an => an
                        .Custom(AUTOCOMPLETE_ANALYZER, c => c
                            .Tokenizer(DEFAULT_TOKENIZER)
                            .Filters(new List<string> { "lowercase", AUTOCOMPLETE_FILTER }))));
            }
        }

        private CreateIndexDescriptor CreateIndexDescriptor(IndexSettingsDescriptor descriptor)
        {
            return new CreateIndexDescriptor(_settings.ClientIndex).Settings(s => descriptor);
        }

        private void CreateIndex(CreateIndexDescriptor descriptor)
        {
            Client.CreateIndex(_settings.ClientIndex, c => descriptor);
        }

        private void AddMappings(CreateIndexDescriptor descriptor)
        {
            var mappingsDescriptor = new MappingsDescriptor();

            // Note that we DO NOT automap these as we want full control over their attributes.
            // Note also that the analyzer and search analyzer are different for displayName.
            mappingsDescriptor.Map(ELASTICSEARCH_DOC_TYPE, m => m
                .Properties(p => p.Text(t => t.Name("segmentKey")))
                .Properties(p => p.Text(t => t.Name("segmentGroup")))
                .Properties(p => p.Text(t => t.Name("displayName").Analyzer(AUTOCOMPLETE_ANALYZER).SearchAnalyzer(DEFAULT_TOKENIZER)))
                .Properties(p => p.Text(t => t.Name("attributes")))
                .Properties(p => p.Text(t => t.Name("tags")))
            );

            descriptor.Mappings(m => mappingsDescriptor);
        }

        #endregion

        // Returns our internal segment document from the infoserv segment document
        // as elastic search stores the entire document.
        private SegmentDocument GetSegmentDocument(ISegment segment)
        {
            return new SegmentDocument
            {
                SegmentKey = segment.Key,
                SegmentGroup = segment.SegmentGroup.Key,
                DisplayName = segment.DisplayName,
                Attributes = string.Empty,
                Tags = string.Empty
            };
        }

        // Adds or updates a document.
        public void AddOrUpdate(ISegment segment)
        {
            // Guarantee that an index already exists. It should, but the check is simple.
            CreateIndexIfNeeded();

            // An update deletes the old document and inserts a new one.
            Delete(segment.Key);

            // Insert the document and make sure the id the same as the segment key. This
            // makes lookup much more efficient later.
            Client.Index<SegmentDocument>(GetSegmentDocument(segment), i => i.Id(segment.Key));

            // EZSTODO - This is costly. Need to find a home for this.
            // Basically elasticsearch will refresh every second which should be OK. However,
            // if running unit tests, the index may not be created by the time we do a search.
            // Hence, we need to do a refresh.
            // Ideally you would like to do a refresh after ALL of the data has been added.
            // That would allow you to do pseudo bulk refreshes. Doing it after every insert
            // slows down the search engine and causes fragmentation issues.
            // For now we can leave it here.
            Client.Refresh(_settings.ClientIndex);
        }

        // Delete the index. Use sparingly.
        public void Clear()
        {
            // Hard reset.
            if (!string.IsNullOrEmpty(_settings.ClientIndex))
            {
                Client.DeleteIndex(_settings.ClientIndex);
            }
        }

        // Deletes the document.
        public void Delete(string segmentKey)
        {
            Client.Delete<SegmentDocument>(segmentKey);
        }

        // Returns a list of matching documents based on the query.
        public SearchResult GetList(
            string query,
            string[] segmentGroupsQuery = null,
            string tagExpression = null,
            uint skip = 0,
            int limit = -1
        )
        {
            // Arbitrary. Need to do something with this. Kind of doesn't make sense to return
            // everything, but who knows. To return everything change value to 0.
            if (limit == -1)
            {
                limit = 1000;
            }

            // Remove this line if you want case insensitive search.
            query = query.ToLowerInvariant();

#if EXACT_MATCH_IMPLEMENTED
            var exactMatch = Client.Search<SegmentDocument>(s => s
                .Index(_settings.ClientIndex)
                .Type(ELASTICSEARCH_DOC_TYPE)
                .Size(limit)
                .Query(q => q.Match(m => m.Field("displayName").Query(query))));
#endif

            // Note that we should never set the "fuzziness" parameter and leave it as auto.
            // It uses the Levenshtein edit distance and can be set to 0, 1, or 2. This will
            // be used on the entire search text regardless of the length of the text.
            // For AUTO setting, the following are allowed based on the length of the search text:
            //      0 - 2: no edits
            //      3 - 5: one edit
            //      5+: two edits
            //
            // Note also that the search below caused great consternation during development as
            // initially it was always returning everything. The problem is related to the .Name()
            // and .Field() attributes. I assumed, incorrectly, that Name specifies the name of the
            // field that you want to search on. What else could it logically be?
            // Well, turns out that name is the name of the resultant set (a key in a dictionary) and
            // to specify which field to use you need the Field attribute.
            var fuzzyMatch = Client.Search<SegmentDocument>(s => s
                .Index(_settings.ClientIndex)
                .Type(ELASTICSEARCH_DOC_TYPE)
                .Size(limit)
                .Query(q => q
                    .Fuzzy(m => m
                        .Name("queryResult")
                        .Field("displayName")
                        .PrefixLength(_settings.FuzzyPrefixLength)
                        .Value(query)
                        )
                    )
                );

            // Store ther results.
            var orderedResults = new List<SearchResultItem>();
            var searchResult = new SearchResult { ResultTotal = fuzzyMatch.Hits.Count, OrderedResults = orderedResults };
            foreach (var document in fuzzyMatch.Hits)
            {
                orderedResults.Add(new SearchResultItem
                {
                    Score = Convert.ToDecimal(document.Score),
                    SegmentKey = document.Source.SegmentKey
                });
            }

            return searchResult;
        }
    }
}
