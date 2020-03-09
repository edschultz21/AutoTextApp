EZSTODO
 - Rename property accordingly in CRMLS_DATA.xml
 - override ToString in Data and Definitions

AutoText wiki:
http://wiki.10kresearch.com:8090/pages/viewpage.action?spaceKey=IN&title=Autotext+-+TenKTextify

 ========
 Positive: > .05%
 Negative: < -.05%
 Flat: otherwise
  
  Words:
  Positive: increased, went up, were up, improved, rose
  Negative: decreased, fell, were down, softened, dropped
  Flat: flat, relatively unchanged, consistent with, relatively/about the same, fairly even
  
  Document Format:
  [INTRO]
  [TEXT FOR UNIT METRICS]
  [TEXT FOR OTHER METRICS]
  [OUTRO]
  
  Paragraph 1
  Metric order: NL, PS, CS, INV
  Format: [PRE] [METRIC NAME] [DIRECTION] [PCT INC/DEC] percent to [ACTUAL VALUE] [CONSECUTIVE TEXT]
  
  Paragraph 2
  a) only 3 metrics?
  b) order by biggest change to smallest change
  c) MSI should almost always be last
  Example: (DOM, MSP, MSI)
  Buyers are snatching up homes as quickly as they are coming onto the market. 
  DOM was down 15.6 percent to 31 days. Median sales price was up 8.2 percent to $190,250. 
  Sellers are excited as MSI was down 12.3 percent to 2.1 months.
  d) Need a list of appropriate lead-in sentences for "good" and "bad" results. (Per comments, skip)
  
  Future:
  =======
  1. Metric - MetricData, MetricDefinition
  2. MetricData - CurrentValue, PreviousValue, PercentChange, ConsecutivePeriods
  3. MetricDefinition - Code (MSP), IsPlural (which verbs to use), ShortName, LongName, IsIncreasePositive
  4. Synonymns - same as words above (see comment about "parallels" in wiki.
  5. Property Type Splits - one paragraph or two? See wiki.
  6. Special Cases: see wiki.
  
  Issues:
  =======
  - Missing (or no) data for current/previous or both. (See ticket)

================================================================================================================
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
    
    One paragraph:
    The overall Median Sales Price was up 1.3 percent to $299,900. 
    The property type with the largest price gain was the Single Family Homes segment, where prices increased 1.3 percent to $405,000. 
    The price range that tended to sell the quickest was the Less than $300,000 range at 87 days; 
    the price range that tended to sell the slowest was the $1,000,000 to $2,000,000 range at 151 days.

    MetricFragment - (eg, New Listings, New Listings and Closed Sales)
    MetricLocationFragment - (eg, "", "in Franklin, Hamilton and Saint Lawrence Counties")
    ChangeFragment - (eg, increased 1.1%, stayed the same, decreased 13.9 percent to $209,000)
    VariableFragment - (eg, for Single Family, for Single Family and Townhouse/Condos)


================================================================================================================
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

================================================================================================================
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

    It may also make sense to move away from words, sentence and paragraph
​    Since there maybe cases when we may create multiple sentences from same piece 

    Like if there are 5 variables in that list, we may need to generate 2 sentences
​    maybe take it even further. say call paragraph a "block" and then just describe what content needs to go into the paragraph
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

    this gives us possibility to phrase it 2 ways. New listings for Single family were X ... while closed sales were Y.
​    vs New listings for Single Family were X, and townhouse condo Z...
​    basically each would generate a "fragment" and then you would assemble fragments into sentences.. some blocks would be one way and some ther other.. just to keep it interesting
​    or if there is 3rd variable, then it would sound better if we talk about all metrics for each variable first.. and then move onto the next one
​    which ends up being 3 paragraphs

More comments from Andrei:
    yeah, we'd have both raw and display values​
    so you would use raw for that, but display values for actual display
​
    i think you should try to abstract it a bit more rather than using functions right from the start
​
    say you have a Iphrase  as the base type​
    then MetricFragment could be class responsible for generating `[DIR] [PCT] percent to [ACTUAL VALUE]`​
    and everything that it needs to do that will be encapsulated there​
    this would allow you to write unit tests for each individual class rather than final result
​
    then you have IFragmentJoiner base type that joins multiple Fragments together​
    TemplateFragmentJoiner takes in template, and Fragment objects (MetricFragment/VariableFragment) and creates sentencefragment
​
    you could also avoid dealing with things as strings, if you just keep fragments in list
    and then implement toSTring at the end

    this way instead of ```
    private string FixDirectionText(string text)
        {
            var dirIndex = text.IndexOf(DIR_TEXT, StringComparison.InvariantCultureIgnoreCase);
            // Sanity check
            if (dirIndex != -1)
            {
                var forIndex = text.IndexOf(" for", dirIndex + 1, StringComparison.InvariantCultureIgnoreCase);
                if (forIndex != -1)
                {
                    var temp = text.Substring(0, dirIndex + DIR_TEXT.Length) + text.Substring(forIndex);
                    return temp;
                }
            }

 

            return text;
        }

    
    you could just create a property "ShowShortForm" or something​
    which will skip "[PCT] percent to [ACTUAL VALUE]"
​
    this is very similar to where we were wtih mdxquery object structures when we started with infoserv
​
    i think we need to keep things as objects all the way until the end and only generate string all the way at the end
​
    say you want a sentence with 3 sentence fragments.. you can implement logic in joiner to do , follwed by and​
    and if there are 2, then just "and" etc​
    you could also access previous sentence to add stuff  like "however" etc


    I guess start off with IFragment <- MetricFragment/VariableFragment
​    then ISentenceFragment <- TemplateFragment / StandardFragment
    and only generate strings when ToString() is called
    even for determining direction, i wouldnt put it into metric fragment

    you could have incoming data->parse into object->process object to determine direction->pass direction directly into metric fragment. 
    this would allow you to create tests with arbitrary direction without worrying about the data

    and for direction words, that should probably come from Provider where you pass in direction and maybe magnitude and it gives you word to use.. 

