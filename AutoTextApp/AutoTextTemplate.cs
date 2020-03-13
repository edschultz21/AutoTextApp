using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public class AutoTextTemplate
    {
        public string Metric { get; set; }
        public string MetricLocation { get; set; }
        public string Data { get; set; }
        public string FlatData { get; set; }
        public string Variable { get; set; }

        public override string ToString()
        {
            return $"{Metric}; {MetricLocation}; {Data}; {FlatData}; {Variable}";
        }
    }
}
