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
            synonyms.Flat = new string[] { "flat", "relatively unchanged", "consistent with", "fairly even" };

            return synonyms;
        }
    }
}
