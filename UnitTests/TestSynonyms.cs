using AutoTextApp;

namespace UnitTests
{
    public class TestSynonyms
    {
        public static Synonyms GetSynonyms()
        {
            var synonyms = new Synonyms();
            synonyms.Positive = new string[] { "increased", "went up", "were up", "improved", "rose" };
            synonyms.Negative = new string[] { "decreased", "fell", "were down", "softened", "dropped" };
            synonyms.Flat = new string[] { "were flat", "were relatively unchanged", "were consistent with", "stayed fairly even" };

            return synonyms;
        }
    }
}
