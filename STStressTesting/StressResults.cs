using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STStressTesting
{
    class StressResults
    {
        public long TotalElapsed { get; set; }
        public int TotalRequests { get; set; }
        public int TotalProblems { get; set; }
        public int TotalExceptions { get; set; }
        public List<long> Results { get; set; }
    }
}
