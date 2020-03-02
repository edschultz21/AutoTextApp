using System.IO;
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

namespace AutoTextApp
{
    public class AutoText
    {
        // EZSTODO - combine these <T>
        public AutoTextDefinition ReadDefinitions(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(AutoTextDefinition));
            return (AutoTextDefinition)serializer.Deserialize(streamReader);
        }

        public AutoTextData ReadData(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(AutoTextData));
            return (AutoTextData)serializer.Deserialize(streamReader);
        }

        public void Run()
        {
            var definitions = ReadDefinitions("Definitions.xml");
            var data = ReadData("CRMLS_Data.xml");
        }
    }
}
