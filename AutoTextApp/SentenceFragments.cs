using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public interface ISentenceFragment
    {
        string GetFragment();
    }

    public class SentenceBuilder : ISentenceFragment
    {
        private List<ISentenceFragment> _sentenceFragments;

        public SentenceBuilder(AutoTextHandlers handlers)
        {
            _sentenceFragments = new List<ISentenceFragment>();
            foreach (var block in handlers.DataHandler.Blocks)
            {
                foreach (var blockItem in block.BlockItems)
                {
                    _sentenceFragments.Add(new StandardFragment(handlers, blockItem));
                }
            }
        }

        public string GetFragment()
        {
            var result = new StringBuilder();

            foreach (var fragment in _sentenceFragments)
            {
                if (fragment != _sentenceFragments.First())
                {
                    result.Append(". ");
                }
                result.Append(fragment.GetFragment());
            }
            if (_sentenceFragments.Count > 0)
            {
                result.Append(".");
            }

            return result.ToString();
        }
    }

    public class TemplateFragment : ISentenceFragment
    {
        // Takes template, fragment objects and creates sentence fragment
        // Create sentence fragments here
        public string GetFragment()
        {
            throw new NotImplementedException();
        }
    }

    public class StandardFragment : ISentenceFragment
    {
        private List<IClauseFragment> _dataFragments;
        private MetricFragment _metricFragment;

        public StandardFragment(AutoTextHandlers handlers, BlockItem blockItem)
        {
            _dataFragments = new List<IClauseFragment>();

            var metricParameters = new MetricFragment.Parameters
            {
                MetricCode = blockItem.MetricCode,
                Template = "[METRIC NAME]"
            };
            _metricFragment = new MetricFragment(handlers, metricParameters);

            var dataParameters = new DataFragment.Parameters
            {
                MetricCode = blockItem.MetricCode,
                Templates = new AutoTextTemplate
                {
                    Data = " [DIR] [PCT] percent to [ACTUAL VALUE]",
                    FlatData = " [DIR]",
                    Variable = " for [ACTUAL NAME]"
                }
            };

            foreach (var variableCode in blockItem.VariableCodes)
            {
                dataParameters.VariableCode = variableCode;
                _dataFragments.Add(new DataFragment(handlers, dataParameters));
            }
        }

        public string GetFragment()
        {
            var result = new StringBuilder();
            result.Append($"{_metricFragment.GetFragment()}");

            foreach (var fragment in _dataFragments)
            {
                if (fragment != _dataFragments.First())
                {
                    result.Append(" and");
                }
                result.Append($"{fragment.GetFragment()}");
            }

            return result.ToString();
        }

        // Takes template, fragment objects and creates sentence fragment
        // Create sentence fragments here
    }
}
