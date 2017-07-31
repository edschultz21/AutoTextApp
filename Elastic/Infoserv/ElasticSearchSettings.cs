namespace Elastic
{
    public class ElasticsearchSettings
    {
        public string Node { get; set; }

        public string ClientIndex { get; set; }

        // This determines how much of the START of the search text that must match EXACTLY.
        // If set to 0, then the whole text is fuzzy matched. This is a somewhat expensive
        // operation. If set to 2, the first two characters MUST match. This acts as a nice
        // filter that speeds up the fuzzy matching.
        public int FuzzyPrefixLength { get; set; }

        public int NumberOfShards { get; set; }

        public int NumberOfReplicas { get; set; }

        public string SynonymPath { get; set; }

        public int MinNgram { get; set; }

        public int MaxNgram { get; set; }
    }
}
