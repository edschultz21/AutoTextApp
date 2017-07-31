using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Elastic
{
    public interface ISegment
    {
        string Key { get; set; }
        string DisplayName { get; set; }

        ISegmentGroup SegmentGroup { get; set; }

        List<string> Tags { get; set; }

        KeyedCollection<string, ISegmentAttribute> Attributes { get; set; }
    }

    public interface ISegmentAttribute
    {
        string Name { get; set; }

        string Value { get; set; }

        SegmentAttributeSearchType SearchType { get; set; }
    }

    public enum SegmentAttributeSearchType
    {
        ALWAYS,
        ONREQUEST,
        NEVER
    }

    public class Segment : ISegment
    {
        public KeyedCollection<string, ISegmentAttribute> Attributes
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string DisplayName { get; set; }

        public string Key { get; set; }

        public ISegmentGroup SegmentGroup { get; set; }

        public List<string> Tags { get; set; }
    }
}
