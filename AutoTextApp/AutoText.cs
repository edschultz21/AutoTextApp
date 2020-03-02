using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

// EZSTODO
// - Rename property accordingly in CRMLS_DATA.xml
// - override ToString in Data and Definitions

// AutoText wiki:
// http://wiki.10kresearch.com:8090/pages/viewpage.action?spaceKey=IN&title=Autotext+-+TenKTextify

/* Current:
 * ========
 * Positive: > .05%
 * Negative: < -.05%
 * Flat: otherwise
 * 
 * Words:
 * Positive: increased, went up, were up, improved, rose
 * Negative: decreased, fell, were down, softened, dropped
 * Flat: flat, relatively unchanged, consistent with, relatively/about the same, fairly even
 * 
 * Document Format:
 * [INTRO]
 * [TEXT FOR UNIT METRICS]
 * [TEXT FOR OTHER METRICS]
 * [OUTRO]
 * 
 * Paragraph 1
 * Metric order: NL, PS, CS, INV
 * Format: [PRE] [METRIC NAME] [DIRECTION] [PCT INC/DEC] percent to [ACTUAL VALUE] [CONSECUTIVE TEXT]
 * 
 * Paragraph 2
 * a) only 3 metrics?
 * b) order by biggest change to smallest change
 * c) MSI should almost always be last
 * Example: (DOM, MSP, MSI)
 * Buyers are snatching up homes as quickly as they are coming onto the market. 
 * DOM was down 15.6 percent to 31 days. Median sales price was up 8.2 percent to $190,250. 
 * Sellers are excited as MSI was down 12.3 percent to 2.1 months.
 * d) Need a list of appropriate lead-in sentences for "good" and "bad" results. (Per comments, skip)
 * 
 * Future:
 * =======
 * 1. Metric - MetricData, MetricDefinition
 * 2. MetricData - CurrentValue, PreviousValue, PercentChange, ConsecutivePeriods
 * 3. MetricDefinition - Code (MSP), IsPlural (which verbs to use), ShortName, LongName, IsIncreasePositive
 * 4. Synonymns - same as words above (see comment about "parallels" in wiki.
 * 5. Property Type Splits - one paragraph or two? See wiki.
 * 6. Special Cases: see wiki.
 * 
 * Issues:
 * =======
 * - Missing (or no) data for current/previous or both. (See ticket)
*/

/*
    Examples:
    =========
    New Listings decreased 11.4 percent for Detached Single-Family homes and 0.8 percent for Attached Single-Family homes. 
    [PRE] - ""
    [METRIC NAME] - New Listings
    [DIRECTION] - decreased
    [PCT CHANGE] - 11.4
    percent to/for
    [ACTUAL VALUE] - ??? Detached Single-Family homes and 0.8 percent for Attached Single-Family homes
    [CONSECUTIVE TEXT] - ""

    Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
    Median Sales Price increased 13.9 percent to $209,000 for Detached Single-Family homes and 14.2 percent to $340,250 for Attached Single-Family homes. 
    Days on Market decreased 3.8 percent for Single Family homes but remained flat for Townhouse/Condo homes. 
    New Listings in Franklin, Hamilton and Saint Lawrence Counties decreased 14.9 percent to 80. Pending Sales decreased 3.4 percent to 57. Inventory decreased 19.7 percent to 763. 
    New Listings remained flat for Single Family but decreased 34.4 percent for Townhouse/Condo properties. 
    Pending Sales increased 65.0 percent for Residential homes and 100.0 percent for Condo homes. 
    Pending Sales decreased 3.4 percent to 57.
*/

/*
    [PRE] [METRIC NAME] [DIRECTION] [PCT INC/DEC] percent to [ACTUAL VALUE] [CONSECUTIVE TEXT]

    Closed Sales increased 1.1 percent for Detached Single-Family homes but decreased 1.0 percent for Attached Single-Family homes. 
    Median Sales Price increased 13.9 percent to $209,000 for Detached Single-Family homes and 14.2 percent to $340,250 for Attached Single-Family homes. 
    Days on Market decreased 3.8 percent for Single Family homes but remained flat for Townhouse/Condo homes. 
    New Listings in Franklin, Hamilton and Saint Lawrence Counties decreased 14.9 percent to 80. Pending Sales decreased 3.4 percent to 57. 
    Inventory decreased 19.7 percent to 763. 
    New Listings remained flat for Single Family but decreased 34.4 percent for Townhouse/Condo properties. 
    Pending Sales increased 65.0 percent for Residential homes and 100.0 percent for Condo homes. 
    Pending Sales decreased 3.4 percent to 57.

    New Listings decreased 11.4 percent for Detached Single-Family homes and 0.8 percent for Attached Single-Family homes. 
    [PRE] - ""
    [METRIC NAME] - New Listings
    [DIRECTION] - decreased
    [PCT CHANGE] - 11.4
    percent to/for
    [ACTUAL VALUE] - ??? Detached Single-Family homes and 0.8 percent for Attached Single-Family homes
    [CONSECUTIVE TEXT] - ""

    CS [inc/dec] [area1 val] percent for [area1] homes [and/but] [inc/dec] [area2 val] percent for [area2] homes
    MSP [inc/dec] [area1 val] percent to [area1 val] for [area1] and [area2 val] percent to [area2 val] for [area2] homes]
    INV [inc/dec] [val] percent to [val]


    INV - [METRIC NAME] [DIRECTION] [PercentChange0] percent to [CurrentValue0]
    MSP - [METRIC NAME] [DIRECTION] [PercentChange0] percent to [CurrentValue0] for [Name0] [and/but] [DIRECTION] [PercentChange1] percent to [CurrentValue1]
    CS  - [METRIC NAME] [DIRECTION] [PercentChange0] percent for [Name0] [and/but] [PercentChange1] percent for [Name1] 
*/
namespace AutoTextApp
{
    public class AutoText
    {
        private Random _random = new Random(381654729);

        public string GetDirection(AutoTextDefinition definitions, bool isPositive, bool isFlat)
        {
            if (isFlat)
            {
                var index = _random.Next(definitions.Synonyms.Flat.Length);
                return definitions.Synonyms.Flat[index];
            } else if (isPositive)
            {
                var index = _random.Next(definitions.Synonyms.Positive.Length);
                return definitions.Synonyms.Positive[index];
            }
            else
            {
                var index = _random.Next(definitions.Synonyms.Negative.Length);
                return definitions.Synonyms.Negative[index];
            }
        }

        public string GetSentenceFragment(AutoTextDefinition definitions, PropertyValue data, string template)
        {
            var metric = definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == data.Name.ToUpper());
            if (metric == null)
            {
                throw new Exception($"Metric not found {data.Name}");
            }

            var result = template;
            var isFlat = data.PercentChange < 0.05;
            var isPositive = (data.CurrentValue - data.PreviousValue) > 0;
            isPositive = metric.IsIncreasePostive ? isPositive : !isPositive;

            var items = Regex.Matches("INV - [METRIC NAME] [DIRECTION] [PercentChange0] percent to [CurrentValue0]", @"\[([^]]*)\]");

            foreach (Match item in items)
            {
                switch (item.Value.ToUpperInvariant())
                {
                    case "[METRIC CODE]":
                        result = result.Replace(item.Value, metric.Code);
                        break;
                    case "[METRIC NAME]":
                        result = result.Replace(item.Value, metric.ShortName);
                        break;
                    case "[METRIC LONGNAME]":
                        result = result.Replace(item.Value, metric.LongName);
                        break;
                    case "[ACTUAL VALUE]":
                        result = result.Replace(item.Value, data.CurrentValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[PREVIOUS VALUE]":
                        result = result.Replace(item.Value, data.PreviousValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[PCT]":
                        result = result.Replace(item.Value, $"{data.PercentChange}%");
                        break;
                    case "[DIR]":
                        result = result.Replace(item.Value, GetDirection(definitions, isPositive, isFlat));
                        break;
                    default:
                        // For now assume it is part of the actual text
                        break;
                }
            }

            return result;
        }

        public T ReadXmlData<T>(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(streamReader);
        }

        public void Run()
        {
            var definitions = ReadXmlData<AutoTextDefinition>("Definitions.xml");
            var data = ReadXmlData<AutoTextData>("CRMLS_Data.xml");

            var results = new Dictionary<PropertyValue, string>();
            foreach (var paragraph in data.Paragraphs)
            {
                foreach (var sentence in paragraph.Sentences)
                {
                    foreach (var value in sentence.PropertyValues)
                    {
                        var result = GetSentenceFragment(definitions, data.Paragraphs[0].Sentences[0].PropertyValues[0], "[METRIC NAME] [DIRECTION] [PercentChange0] percent to [CurrentValue0]");
                        results.Add(value, result);
                    }
                }
            }
            //GetSentenceFragment(definitions, data.Paragraphs[0].Sentences[0].PropertyValues[0], "[METRIC NAME] [DIRECTION] [PercentChange0] percent to [CurrentValue0]");
        }
    }
}
