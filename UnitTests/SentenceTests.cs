using AutoTextApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SentenceTests
    {
        private IFragmentGenerators _generator;
        private IDefinitionProvider _definitionProvider;
        private IDataProvider _dataProvider;
        private IMacroVariables _macroVariables;
        private IRenderer _renderer;

        [TestInitialize]
        public void TestInitialize()
        {
            _definitionProvider = new TestDefinitionProvider();
            _dataProvider = new TestDataProvider();
            _generator = new FragmentGenerators(_definitionProvider, _dataProvider);
            _macroVariables = new TestMacroVariables();
            var direction = new Direction(TestSynonyms.GetSynonyms());
            _renderer = new Basic10KRenderer(_macroVariables, direction);
        }

        private BlockItem GetBlockItem(string metricCode, string[] variableCodes)
        {
            return new BlockItem
            {
                MetricCode = metricCode,
                VariableCodes = variableCodes
            };
        }

        private Block GetBlock(BlockItem[] blockItems)
        {
            return new Block
            {
                BlockType = BlockType.Sentence,
                BlockItems = blockItems
            };
        }

        private void ValidateDataFragment(DataFragment dataFragment, string variableCode, DirectionType direction)
        {
            Assert.AreEqual(variableCode, dataFragment.VariableCode);
            Assert.AreEqual(direction, dataFragment.Direction);
        }

        [TestMethod]
        public void OneVariable()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("NL", new string[] { "SF" }),
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("New Listings were down 7.5 percent to 82.5 for Single Family", result);
        }

        [TestMethod]
        public void TwoVariable()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("NL", new string[] { "SF" }),
                    GetBlockItem("CS", new string[] { "TC", "SF" })
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("New Listings were down 7.5 percent to 82.5 for Single Family. Closed Sales stayed fairly even for Townhouse/Condo and stayed fairly even for Single Family.", result);
        }

        //[TestMethod]
        //public void BlockItem1()
        //{
        //    var sentence = new Sentence();
        //    var sentenceFragment = sentence.BuildSentenceFragment(_generator, GetBlockItem("NL", new string[] { "SF", "TC" }));

        //    Assert.AreEqual("NL", sentenceFragment.MetricFragment.MetricCode);
        //    Assert.AreEqual(2, sentenceFragment.DataFragments.Count);

        //    ValidateDataFragment(sentenceFragment.DataFragments[0], "SF", DirectionType.NEGATIVE);
        //    ValidateDataFragment(sentenceFragment.DataFragments[1], "TC", DirectionType.POSITIVE);
        //}

        //[TestMethod]
        //public void Block1()
        //{
        //    var sentence = new Sentence();
        //    var blockItems = new BlockItem[] {
        //        GetBlockItem("NL", new string[] { "SF", "TC" }),
        //        GetBlockItem("CS", new string[] { "TC", "SF" })
        //    };

        //    sentence.BuildSentenceFragments(_generator,
        //        new Block[]
        //        {
        //            GetBlock(blockItems)
        //        });

        //    Assert.AreEqual(2, sentence.SentenceFragments.Count);

        //    var sentenceFragment = sentence.SentenceFragments[0];
        //    Assert.AreEqual("NL", sentenceFragment.MetricFragment.MetricCode);
        //    ValidateDataFragment(sentenceFragment.DataFragments[0], "SF", DirectionType.NEGATIVE);
        //    ValidateDataFragment(sentenceFragment.DataFragments[1], "TC", DirectionType.POSITIVE);

        //    sentenceFragment = sentence.SentenceFragments[1];
        //    Assert.AreEqual("CS", sentenceFragment.MetricFragment.MetricCode);
        //    ValidateDataFragment(sentenceFragment.DataFragments[0], "TC", DirectionType.FLAT);
        //    ValidateDataFragment(sentenceFragment.DataFragments[1], "SF", DirectionType.FLAT);
        //}

        //[TestMethod]
        //public void Block2()
        //{
        //    var sentence = new Sentence();
        //    var blockItems1 = new BlockItem[] {
        //        GetBlockItem("NL", new string[] { "SF", "TC" }),
        //        GetBlockItem("CS", new string[] { "TC", "SF" })
        //    };
        //    var blockItems2 = new BlockItem[] {
        //        GetBlockItem("NL", new string[] { "SF", "MB" }),
        //        GetBlockItem("MSI", new string[] { })
        //    };

        //    sentence.BuildSentenceFragments(_generator,
        //        new Block[]
        //        {
        //            GetBlock(blockItems1),
        //            GetBlock(blockItems2)
        //        });
        //}
    }
}
