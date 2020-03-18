namespace AutoTextApp
{

    public class Templates
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
