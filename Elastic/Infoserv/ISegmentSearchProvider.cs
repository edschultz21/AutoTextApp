using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastic
{
    public interface ISegmentSearchProvider
    {
        /// <summary>
        /// Adds segment to the search provider, or updates existing segment if it exists
        /// </summary>
        /// <param name="segment"></param>
        void AddOrUpdate(ISegment segment);

        /// <summary>
        /// Queries segments from the search provider
        /// </summary>
        /// <param name="query"></param>
        /// <param name="segmentGroupsQuery"></param>
        /// <param name="tagExpression"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        SearchResult GetList(string query, string[] segmentGroupsQuery = null, string tagExpression = null, uint skip = 0, int limit = -1);

        /// <summary>
        /// Clear all segments for the search provider
        /// </summary>
        void Clear();

        /// <summary>
        /// Delete segment from the segment search
        /// </summary>
        /// <param name="segment"></param>
        void Delete(string segmentKey);
    }

    public class SearchResult
    {
        public int ResultTotal { get; set; }

        public IEnumerable<SearchResultItem> OrderedResults { get; set; }

    }

    public class SearchResultItem
    {
        public string SegmentKey { get; set; }
        public decimal Score { get; set; }

        public override string ToString()
        {
            return $"{SegmentKey}: {Score}";
        }
    }

}
