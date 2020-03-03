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

/*
 Comments from Andrei:
    so i was thinking, maybe we could pass data to the service in pre-defined structure
​    kinda like we do in annual, ie.. i1...iN different indices give you a data point, previous, percent change, consecutive
​    then you can refer to them in the actual "structure" doc
​    so you would end up with something like this: 
​
    "Paragraph": [
    {
        "Type": "simple",
        "MetricCode": "NL",
        "Variables": [
            "SF",
            "TC"
        ]
    },
    {
        "Type": "simple",
        "MetricCode": "CS",
        "Variables": [
            "SF",
            "TC"
        ]
    }
    ]

    It may also make sense to move away from words sentence and paragraph
​    Since there maybe cases when we may create multiple sentences from same piece 

    Like if there are 5 variables in that list, we may need to generate 2 sentences
​    maybe take it even further.. say call paragraph a "block" and then just describe what content needs to go into the paragraph
​    and then the service would figure out how to describe it
​    
    "Block": [
        {        
            "MetricCode": "NL",
            "Variables": "SF"
        },
        {        
            "MetricCode": "NL",
            "Variables": "TC"
        },
        {        
            "MetricCode": "CS",
            "Variables": "SF"
        },
        {        
            "MetricCode": "CS",
            "Variables": "TC"
        },
    ]

    this gives us possibility to phrase it 2 ways.. New listings for Single family were X ... while closed sales were Y.
​    vs New listings for Single Family were X, and townhouse condo Z...
​    basically each would generate a "fragment" and then you would assemble fragments into sentences.. some blocks would be one way and some ther other.. just to keep it interesting
​    or if there is 3rd variable, then it would sound better if we talk about all metrics for each variable first.. and then move onto the next one
​    which ends up being 3 paragraphs

 */
namespace AutoTextApp
{
    public class AutoText
    {
        private void SetDirection(AutoTextDefinition definitions, AutoTextData data)
        {
            foreach (var paragraph in data.Paragraphs)
            {
                foreach (var sentence in paragraph.Sentences)
                {
                    foreach (var propertyValue in sentence.PropertyValues)
                    {
                        propertyValue.Direction = DirectionType.FLAT;
                        if (propertyValue.PercentChange >= 0.05)
                        {
                            var isPositive = (propertyValue.CurrentValue - propertyValue.PreviousValue) > 0;

                            var metric = definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == sentence.Code.ToUpper());
                            if (metric != null)
                            {
                                isPositive = metric.IsIncreasePostive ? isPositive : !isPositive;
                            }
                            
                            propertyValue.Direction = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
                        }
                    }
                }
            }
        }

        private string GetDirection(AutoTextDefinition definitions, DirectionType direction, Random random)
        {
            if (direction == DirectionType.FLAT)
            {
                var index = random.Next(definitions.Synonyms.Flat.Length);
                return definitions.Synonyms.Flat[index];
            }
            else if (direction == DirectionType.POSITIVE)
            {
                var index = random.Next(definitions.Synonyms.Positive.Length);
                return definitions.Synonyms.Positive[index];
            }
            else // direction == DirectionType.NEGATIVE
            {
                var index = random.Next(definitions.Synonyms.Negative.Length);
                return definitions.Synonyms.Negative[index];
            }
        }

        private string GetSentenceFragment(AutoTextDefinition definitions, Dictionary<string, string> variables, string metricCode, PropertyValue propertyValue, string template, int seed)
        {
            var random = new Random(seed);
            var metric = definitions.Metrics.FirstOrDefault(x => x.Code.ToUpper() == metricCode.ToUpper());
            if (metric == null)
            {
                //throw new Exception($"Metric not found {data.Name}"); - EZSTODO
                return $"Metric not found {propertyValue.Name}";
            }

            var result = template;

            var items = Regex.Matches(template, @"\[([^]]*)\]");

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
                        result = result.Replace(item.Value, propertyValue.CurrentValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[PREVIOUS VALUE]":
                        result = result.Replace(item.Value, propertyValue.PreviousValue.ToString()); // EZSTODO - need to figure out formatting (eg, 579000 -> $579,000)
                        break;
                    case "[ACTUAL NAME]":
                        result = result.Replace(item.Value, propertyValue.Name);
                        break;
                    case "[PCT]":
                        result = result.Replace(item.Value, $"{propertyValue.PercentChange}%");
                        break;
                    case "[DIR]":
                        result = result.Replace(item.Value, GetDirection(definitions, propertyValue.Direction, random));
                        break;
                    default:
                        if (variables.ContainsKey(item.Value))
                        {
                            result = result.Replace(item.Value, variables[item.Value]);
                        }
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
            Random random = new Random(381654729);

            var definitions = ReadXmlData<AutoTextDefinition>("Definitions.xml");
            var data = ReadXmlData<AutoTextData>("CRMLS_Data.xml");
            SetDirection(definitions, data);
            var variables = definitions.Variables?.ToDictionary(x => $"[{x.Name}]", y => string.IsNullOrEmpty(y?.Value) ? "" : y.Value) ?? new Dictionary<string, string>();

            var results = new Dictionary<PropertyValue, string>();
            foreach (var paragraph in data.Paragraphs)
            {
                // EZSTODO - sort sentences by percent change (largest to smallest)
                foreach (var sentence in paragraph.Sentences)
                {
                    var seed = random.Next(int.MaxValue);

                    // EZSTODO - sort property values by percent change (largest to smallest)
                    // - need to handle conjunction correctly. Above needs to be sorted by pos/neg change. All positive/negatives are
                    // "and"ed while the two are "or"ed.
                    foreach (var propertyValue in sentence.PropertyValues)
                    {
                        var result = GetSentenceFragment(definitions, variables, sentence.Code, propertyValue, "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE] for [ACTUAL NAME][HOMES]", seed);
                        results.Add(propertyValue, result);
                    }
                }
            }
            //var ezs = GetSentenceFragment(definitions, data.Paragraphs[0].Sentences[0].Code, data.Paragraphs[0].Sentences[0].PropertyValues[0], "[METRIC NAME] [DIR] [PCT] percent to [ACTUAL VALUE]");
        }
    }
}
