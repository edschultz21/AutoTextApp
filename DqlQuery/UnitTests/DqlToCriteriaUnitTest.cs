using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DqlQuery;
using DqlHelpers;
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
            using (ISession session = SessionHelper.OpenSession())
            {
                var criteria = (ICriteria)dqlMain.GetCriteria(text, session);
                var sql = GenerateSql.GetSql(criteria);
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

        [TestMethod]
        public void DqlToCriteria_Test8()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s, s.Agent a " +
                "WHERE a.Age  = 45";

            // Does not throw exception.
            var results = RunCriteria(text);
        }

        [TestMethod]
        public void DqlToCriteria_TrimFunction()
        {
            // FAIL case (does not match)
            string text =
                "GET Listing " +
                "FROM Listing l " +
                "WHERE rtrim(' ezs ') = 'ezs '";
            var results = RunCriteria(text);
            Assert.IsTrue(results == "");

            // SUCCESS case (matches)
            text =
                "GET Listing " +
                "FROM Listing l " +
                "WHERE rtrim(' ezs ') = ' ezs'";
            results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 4 SELL) L3 3 300000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000### L6 6 450000###( 15 BUY) L7 7 356789###( 9 POKE) L8 8 420000### L9 9 565000###( 7 POKE)( 8 PEEK) L10 10###");
        }

        [TestMethod]
        public void DqlToCriteria_CustomFunction()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s, s.Agent a " +
                "WHERE a.Age  = powertestfunc(7, 2) + 3";
            var results = RunCriteria(text);
            Assert.IsTrue(results == "(()(()Agent  Listing L5Agent  Listing 5Agent  Listing 100000Agent  14Agent  OTHER)Agent 8Agent 52Agent EDWARDAgent Eddie Miller 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###((()(Agent  Listing  11Agent  Listing  wait)(Agent  Listing  12Agent  Listing  Wait)Agent  Listing L4Agent  Listing 4Agent  Listing 255000Agent  10Agent  WAIT)()Agent 8Agent 52Agent EDWARDAgent Eddie Miller 14 OTHER) L5 5 100000###");
        }

        [TestMethod]
        public void CompExprPropertyValue()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice < 300000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000###");
        }

        [TestMethod]
        public void CompExprValueProperty()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE 300000 > l.ListPrice";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000###");
        }

        [TestMethod]
        public void CompExprProjectionValue()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice / 10 < 30000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000###");
        }

        [TestMethod]
        public void CompExprValueProjection()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE 30000 > l.ListPrice / 10";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 3 BUY)( 6) L2 2 200000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###( 14 OTHER) L5 5 100000###");
        }

        [TestMethod]
        public void CompExprProjectionProperty()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE l.ListPrice / 100000 = s.Agent";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###");
        }

        [TestMethod]
        public void CompExprPropertyProjection()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Agent = l.ListPrice / 100000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###");
        }

        [TestMethod]
        public void CompExprPropertyProperty()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Agent = s.Listing";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 10 WAIT)( 11 wait)( 12 Wait) L4 4 255000###");
        }

        [TestMethod]
        public void CompExprProjectionProjection()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Agent - 1 = l.ListPrice / 100000";

            var results = RunCriteria(text);
            Assert.IsTrue(results == "( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###( 1 SELL)( 2 SELL)( 5 BUY) L1 1 100000###");
        }
    }
}
