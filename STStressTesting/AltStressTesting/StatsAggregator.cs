using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STStressTesting
{
    public class StatsAggregator
    {
        private ConcurrentBag<Tuple<string,DateTime,string>> _bag = new ConcurrentBag<Tuple<string, DateTime, string>>();
        private DateTime _startTime;

        public StatsAggregator()
        {
            _startTime = DateTime.Now;
        }

        public void RecordError(string result)
        {
            _bag.Add(new Tuple<string, DateTime, string>("ERROR",DateTime.Now, result));
        }

        public void RecordSuccess(long timerElapsedMilliseconds)
        {
            _bag.Add(new Tuple<string, DateTime, string>("OK", DateTime.Now, timerElapsedMilliseconds.ToString()));
        }

        public override string ToString()
        {
            var duration = DateTime.Now - _startTime;

            var totalRuns = _bag.Count;
            var sucesses = _bag.Count(t => t.Item1 == "OK");
            var errors = _bag.Count(t => t.Item1 == "ERROR");
            var successTimes = _bag.Where(t => t.Item1 == "OK").Select(t => Int32.Parse(t.Item3)).ToList();
            var averageTime = successTimes.Average();
            var minTime = successTimes.Min();
            var maxTime = successTimes.Max();

            return $"\nDuration: {duration.TotalSeconds} seconds\n" +
                   $"Requests Total: {totalRuns}\n" +
                   $"Success: {sucesses} Errors: {errors}\n" +
                   $"Success Rate: {sucesses / duration.TotalSeconds * 60} CPM\n" +
                   $"Response Time: Min={minTime} Avg={averageTime} Max={maxTime}";
        }
    }
}
