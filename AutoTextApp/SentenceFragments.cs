using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                // EZSTODO - BlockType 
                //   Sentence - Keep as is and do not change order
                //   Auto - Reorganize as needed
                foreach (var blockItem in block.BlockItems)
                {
                    if (blockItem.Templates != null)
                    {
                        _sentenceFragments.Add(new TemplateFragment(handlers, blockItem, blockItem.Templates));
                    }
                    else
                    {
                        _sentenceFragments.Add(new StandardFragment(handlers, blockItem));
                    }
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
        private List<IClauseFragment_Old> _dataFragments;
        private MetricFragment_Old _metricFragment;

        public TemplateFragment(AutoTextHandlers handlers, BlockItem blockItem, AutoTextTemplate templates)
        {
            _dataFragments = new List<IClauseFragment_Old>();

            var metricParameters = new MetricFragment_Old.Parameters
            {
                MetricCode = blockItem.MetricCode,
                Template = templates.Metric
            };
            _metricFragment = new MetricFragment_Old(handlers, metricParameters);

            var dataParameters = new DataFragment_Old.Parameters
            {
                MetricCode = blockItem.MetricCode,
                Templates = templates
            };

            if (blockItem.VariableCodes == null || blockItem.VariableCodes.Length == 0)
            {
                dataParameters.VariableCode = null;
                _dataFragments.Add(new DataFragment_Old(handlers, dataParameters));
            }
            else
            {
                foreach (var variableCode in blockItem.VariableCodes)
                {
                    dataParameters.VariableCode = variableCode;
                    _dataFragments.Add(new DataFragment_Old(handlers, dataParameters));
                }
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
    }

    public class StandardFragment : ISentenceFragment
    {
        private TemplateFragment _templateFragment;

        public StandardFragment(AutoTextHandlers handlers, BlockItem blockItem)
        {
            var templates = new AutoTextTemplate
            {
                Metric = "[METRIC NAME]",
                Data = " [DIR] [PCT] percent to [ACTUAL VALUE]",
                FlatData = " [DIR]",
                Variable = " for [ACTUAL NAME]"
            };
            _templateFragment = new TemplateFragment(handlers, blockItem, templates);
        }

        public string GetFragment()
        {
            return _templateFragment.GetFragment();
        }
    }
}
