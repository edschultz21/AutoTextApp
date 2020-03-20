
using System.Collections.Generic;
using System.Text;

namespace AutoTextApp
{
    public interface IRenderer
    {
        string RenderData(ISentences sentences);
    }

    public class Basic10KRenderer : IRenderer
    {
        private IMacroVariables _macroVariable;
        private IDirection _direction;
        private Templates _templates;

        private readonly Templates DefaultTemplates = new Templates
        {
            Metric = "[METRIC NAME]",
            Data = "[DIR] [PCT] percent to [ACTUAL VALUE][DATA UNITS]",
            FlatData = "[DIR]",
            Variable = " for [ACTUAL NAME][VAR UNITS]"
        };

        public Basic10KRenderer(IMacroVariables macroVariable, Templates templates, IDirection direction)
        {
            _macroVariable = macroVariable;
            _direction = direction;
            _templates = templates ?? DefaultTemplates;
        }

        private string Process(IClauseFragment fragment, string template)
        {
            return MacroVariableProcessor.ProcessMacros(_macroVariable, fragment, template);
        }

        private List<string> GetConjugations(List<DataFragment> dataFragments)
        {
            var conjugations = new List<string>();

            if (dataFragments.Count == 2)
            {
                // Let's keep this as simple as possible for now
                if (dataFragments[0].Direction == dataFragments[1].Direction)
                {
                    conjugations.Add(" and");
                }
                else if (dataFragments[0].Direction == DirectionType.FLAT || dataFragments[1].Direction == DirectionType.FLAT)
                {
                    conjugations.Add(" while it");
                }
                else
                {
                    conjugations.Add(" but");
                }
            }
            else if (dataFragments.Count > 2)
            {
                // EZSTODO - need to handle better (dataFragments.Count > 2)
                for (int i = 0; i < dataFragments.Count - 1; i++)
                {
                    conjugations.Add(" and");
                }
            }

            return conjugations;
        }

        private string ProcessSegment(ISentenceFragment sentenceFragment)
        {
            StringBuilder result = new StringBuilder();

            result.Append(Process(sentenceFragment.MetricFragment, _templates.Metric));

            var conjugations = GetConjugations(sentenceFragment.DataFragments);
            for (int i = 0; i < sentenceFragment.DataFragments.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(conjugations[i - 1]);
                }

                var dataFragment = sentenceFragment.DataFragments[i];
                dataFragment.Units = sentenceFragment.MetricFragment.Metric.Units;

                // Trivial optimization (works only for two variables).
                // Instead of "were flat for A and were flat for B" we now get "were flat for A and B".
                if (i != 0 && dataFragment.Direction == DirectionType.FLAT && sentenceFragment.DataFragments[i - 1].Direction == DirectionType.FLAT)
                {
                    _macroVariable.Get("DIR").Value = "";
                }
                else
                {
                    _macroVariable.Get("DIR").Value = " " + _direction.GetDirectionText(dataFragment.Direction);
                }

                var template = dataFragment.Direction == DirectionType.FLAT ? _templates.FlatData : _templates.Data;
                result.Append(Process(dataFragment, template));
                result.Append(Process(dataFragment.VariableFragment, _templates.Variable));
            }

            return result.ToString();
        }

        public string RenderData(ISentences sentences)
        {
            StringBuilder result = new StringBuilder();

            foreach (var sentenceFragment in sentences.SentenceFragments)
            {
                result.Append(ProcessSegment(sentenceFragment));
                result.Append(". ");
            }

            return result.ToString().TrimEnd(' ');
        }
    }
}
