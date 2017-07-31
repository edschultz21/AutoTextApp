using System;
using System.Collections.Generic;

namespace Elastic
{
    public interface ISegmentGroup
    {
        string Key { get; set; }

        //List<string> SearchList { get; }
    }

    public class SegmentGroup : ISegmentGroup
    {
        public string Key { get; set; }
    }
}