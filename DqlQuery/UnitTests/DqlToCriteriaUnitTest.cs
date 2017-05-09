using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DqlQuery;
using NHibernate;

namespace UnitTests
{
    /// <summary>
    /// Summary description for DqlToCriteria
    /// </summary>
    [TestClass]
    public class DqlToCriteriaUnitTest
    {

        private string RunCriteria(string text)
        {
            var output = string.Empty;

            var dqlMain = new DqlMain();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var criteria = (ICriteria)dqlMain.GetCriteria(text, session);
                var sql = DqlQuery.TestItems.GenerateSql.GetSql(criteria);
                criteria.SetFetchMode("Listing", FetchMode.Eager);
                IList results = criteria.List();

                output = new OutputRows(false).GetValues(results);
            }

            return output;
        }


        [TestMethod]
        public void DqlToCriteria_Test1()
        {
            string text =
                "GET Listing " +
                "FROM Listing l";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 4 SELL) L3 3 300000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000### L6 6 450000###( 15 BUY) L7 7 356789###( 9 POKE) L8 8 420000### L9 9 565000###( 7 POKE)( 8 PEEK) L10 10###");
        }

        [TestMethod]
        public void DqlToCriteria_Test2()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL'";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 4 SELL) L3 3 300000###");
        }

        [TestMethod]
        public void DqlToCriteria_Test3()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL' OR s.Side LIKE 'P%'";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 4 SELL) L3 3 300000###( 7 POKE)( 8 PEEK) L10 10###( 7 POKE)( 8 PEEK) L10 10###( 9 POKE) L8 8 420000###");
        }

        [TestMethod]
        public void DqlToCriteria_Test4()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice BETWEEN 200000 and 450000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 3 BUY)( 6) L2 2 200000###( 4 SELL) L3 3 300000###( 3 BUY)( 6) L2 2 200000###( 9 POKE) L8 8 420000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 15 BUY) L7 7 356789###");
        }

        [TestMethod]
        public void DqlToCriteria_Test5()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice IS NULL";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 7 POKE)( 8 PEEK) L10 10###( 7 POKE)( 8 PEEK) L10 10###");
        }

        [TestMethod]
        public void DqlToCriteria_Test6()
        {
            string text =
                "GET Listing.* " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice > 100000 + 200001 " +
                "ORDER BY l.ListPrice DESC ";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 9 POKE) L8 8 420000###( 15 BUY) L7 7 356789###");
        }

        [TestMethod]
        public void DqlToCriteria_Test7()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL' AND l.ListPrice <= 200000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###");
        }
    }
}
