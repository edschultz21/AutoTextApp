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
            _renderer = new Basic10KRenderer(_macroVariables, _definitionProvider.Templates, direction);
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
        public void NoVariable()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("MSI", new string[]{ }),
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("MSI were up 12.3 percent to 2.4 months.", result);
        }

        [TestMethod]
        public void OneVariable_NL1()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("NL", new string[] { }),
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("New Listings were down 7 percent to 12.3.", result);
        }

        [TestMethod]
        public void OneVariable_NL2()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("NL", new string[] { }),
                    GetBlockItem("NL", new string[] { "SF" }),
                    GetBlockItem("NL", new string[] { "MB", "SF" }),
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("New Listings were down 7 percent to 12.3. New Listings softened 7.5 percent to 82.5 for Single Family homes. New Listings rose 17.5 percent to 164.7 for Mobile but fell 7.5 percent to 82.5 for Single Family homes.", result);
        }

        [TestMethod]
        public void AllVariable()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("NL", new string[] { "MB", "SF", "TC" }),
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("New Listings were up 17.5 percent to 164.7 for Mobile and softened 7.5 percent to 82.5 for Single Family homes and rose 7.5 percent to 92.5 for Townhouse/Condo homes.", result);
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
            Assert.AreEqual("New Listings were down 7.5 percent to 82.5 for Single Family homes.", result);
        }

        [TestMethod]
        public void TwoVariable_BothFlat()
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
            Assert.AreEqual("New Listings were down 7.5 percent to 82.5 for Single Family homes. Closed Sales stayed fairly even for Townhouse/Condo homes and for Single Family homes.", result);
        }

        [TestMethod]
        public void TwoVariable_OneFlat()
        {
            var blockItems = new BlockItem[] {
                    GetBlockItem("MSP", new string[] { "SF", "TC" })
                };

            var sentences = new Sentences();
            sentences.BuildSentenceFragments(_generator,
                new Block[]
                {
                        GetBlock(blockItems)
                });

            var result = _renderer.RenderData(sentences);
            Assert.AreEqual("Median Sales Price were relatively unchanged for Single Family homes while it improved 13.9 percent to 209000 for Townhouse/Condo homes.", result);
        }
    }
}
