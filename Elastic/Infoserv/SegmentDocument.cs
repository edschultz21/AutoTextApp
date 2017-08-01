using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastic
{
    public class SegmentDocument
    {
        public string SegmentKey { get; set; }
        public string SegmentGroup { get; set; }
        public string DisplayName { get; set; }

        public string Attributes { get; set; } // EZSTODO - Figure out proper type. Nested?
        public string Tags { get; set; } // EZSTODO - Figure out proper type. Nested?
    }
}
