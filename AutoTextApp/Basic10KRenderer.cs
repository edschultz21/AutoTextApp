
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

        private readonly Templates _defaultTemplates = new Templates
        {
            Metric = "[METRIC NAME]",
            Data = " [DIR] [PCT] percent to [ACTUAL VALUE]",
            FlatData = " [DIR]",
            Variable = " for [ACTUAL NAME]"
        };

        public Basic10KRenderer(IMacroVariables macroVariable, IDirection direction)
        {
            _macroVariable = macroVariable;
            _direction = direction;
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
                    conjugations.Add(" while");
                }
                else
                {
                    conjugations.Add(" but");
                }
            }
            else if (dataFragments.Count > 2)
            {
                // EZSTODO - need to handle (dataFragments.Count > 2)
            }

            return conjugations;
        }

        private string ProcessSegment(ISentenceFragment sentenceFragment)
        {
            StringBuilder result = new StringBuilder();

            result.Append(Process(sentenceFragment.MetricFragment, _defaultTemplates.Metric));

            var conjugations = GetConjugations(sentenceFragment.DataFragments);
            for (int i = 0; i < sentenceFragment.DataFragments.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(conjugations[i - 1]);
                }

                var dataFragment = sentenceFragment.DataFragments[i];
                _macroVariable.Get("DIR").Value = _direction.GetDirectionText(dataFragment.Direction);

                var template = dataFragment.Direction == DirectionType.FLAT ? _defaultTemplates.FlatData : _defaultTemplates.Data;
                result.Append(Process(dataFragment, template));
                result.Append(Process(dataFragment.VariableFragment, _defaultTemplates.Variable));
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
