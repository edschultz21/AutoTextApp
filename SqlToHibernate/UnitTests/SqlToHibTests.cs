using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToHibernate;

namespace UnitTests
{
    [TestClass]
    public class SqlToHibTests
    {
        [TestMethod]
        public void SqlToHib_Test1()
        {
            string text =
                "GET Entity " +
                "FROM Entity e2 " +
                "WHERE NOT Q=5 and R=6 AND S=7";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Entity FROM Entity e2 .Add(Restrictions.Conjunction().Add(Restrictions.Not(Restrictions.Eq(\"Q\", 5))).Add(Restrictions.Eq(\"R\", 6)).Add(Restrictions.Eq(\"S\", 7)))");
        }

        [TestMethod]
        public void SqlToHib_Test2()
        {
            string text =
                "GET Entity.Quantity " +
                "FROM Entity " +
                "WHERE Q = 55 OR Q <= 35 AND Q >= 7";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Entity.Quantity FROM Entity .Add(Restrictions.Disjunction().Add(Restrictions.Eq(\"Q\", 55)).Add(Restrictions.Conjunction().Add(Restrictions.Le(\"Q\", 35)).Add(Restrictions.Ge(\"Q\", 7))))");
        }

        [TestMethod]
        public void SqlToHib_Test3()
        {
            string text =
                "GET Entity1.Quantity, IsTart " +
                "FROM Entity2 e2 " +
                "WHERE Q = 55 OR Q <= 35 AND Q >= 7 " +
                "ORDER BY Quantity ASC, IsTart DESC " +
                "SKIP 10";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Entity1.Quantity,IsTart FROM Entity2 e2 .Add(Restrictions.Disjunction().Add(Restrictions.Eq(\"Q\", 55)).Add(Restrictions.Conjunction().Add(Restrictions.Le(\"Q\", 35)).Add(Restrictions.Ge(\"Q\", 7)))) " +
                ".AddOrder(Order.Asc(\"Quantity\")).AddOrder(Order.Desc(\"IsTart\")) .SetFirstResult(10)");
        }

        [TestMethod]
        public void SqlToHib_Test4()
        {
            string text =
                "GET Entity1.Quantity, IsTart " +
                "FROM Entity2 " +
                "WHERE Q <= 55 " +
                "ORDER BY Quantity ASC " +
                "TAKE 10";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Entity1.Quantity,IsTart FROM Entity2 .Add(Restrictions.Le(\"Q\", 55)) .AddOrder(Order.Asc(\"Quantity\")) .SetMaxResults(10)");
        }

        [TestMethod]
        public void SqlToHib_Test5()
        {
            string text =
                "GET IsTart " +
                "FROM e2 " +
                "WHERE Q IS NOT NULL " +
                "SKIP 10 TAKE 20";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart FROM e2 .Add(Restrictions.IsNotNull(\"Q\"))  .SetFirstResult(10).SetMaxResults(20)");
        }

        [TestMethod]
        public void SqlToHib_Test6()
        {
            string text =
                "GET IsTart " +
                "FROM e2 " +
                "WHERE Q IS NOT NULL " +
                "TAKE 20 SKIP 10";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart FROM e2 .Add(Restrictions.IsNotNull(\"Q\"))  .SetFirstResult(10).SetMaxResults(20)");
        }

        [TestMethod]
        public void SqlToHib_Test7()
        {
            string text =
                "GET IsTart " +
                "FROM e2 " +
                "WHERE Q=10 AND R<=20 OR T <> 'ST'";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(Restrictions.Eq(\"Q\", 10)).Add(Restrictions.Le(\"R\", 20))).Add(Restrictions.Not(Restrictions.Eq(\"T\", 'ST'))))");
        }

        [TestMethod]
        public void SqlToHib_Test8()
        {
            string text =
                "GET IsTart " +
                "FROM e2 " +
                "WHERE Q=10 OR R!=20 AND T <> 'ST'";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Eq(\"Q\", 10)).Add(Restrictions.Conjunction().Add(Restrictions.Not(Restrictions.Eq(\"R\", 20))).Add(Restrictions.Not(Restrictions.Eq(\"T\", 'ST')))))");
        }

        [TestMethod]
        public void SqlToHib_Test9()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q=10 OR R!=20 AND T NOT BETWEEN 7 AND 20";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Eq(\"Q\", 10)).Add(Restrictions.Conjunction().Add(Restrictions.Not(Restrictions.Eq(\"R\", 20))).Add(Restrictions.Not(Restrictions.Between(\"T\", 7, 20)))))");
        }

        [TestMethod]
        public void SqlToHib_Test10()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q>=10 AND R< 20 OR T BETWEEN 7 AND 20";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(Restrictions.Ge(\"Q\", 10)).Add(Restrictions.Lt(\"R\", 20))).Add(Restrictions.Between(\"T\", 7, 20)))");
        }

        [TestMethod]
        public void SqlToHib_Test11()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q>=10 / 2 + 7 AND R< 20 OR T BETWEEN 7 AND 20 + 10 * 4";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(Restrictions.Ge(\"Q\", ((10/2)+7))).Add(Restrictions.Lt(\"R\", 20))).Add(Restrictions.Between(\"T\", 7, (20+(10*4)))))");
        }

        [TestMethod]
        public void SqlToHib_Test12()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q>= -10 / (2 + 7) AND R< 20 OR T BETWEEN 7 AND 20 + (10 * 4)";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(Restrictions.Ge(\"Q\", (-10/((2+7))))).Add(Restrictions.Lt(\"R\", 20))).Add(Restrictions.Between(\"T\", 7, (20+((10*4))))))");
        }

        [TestMethod]
        public void SqlToHib_Test13()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q IN ('Active', 'Closed')";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.In(\"Q\", ('Active','Closed')))");
        }

        [TestMethod]
        public void SqlToHib_Test14()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q IN ('Active')";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.In(\"Q\", ('Active')))");
        }

        [TestMethod]
        public void SqlToHib_Test15()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Q IN ('Active', 'Closed') AND R BETWEEN 7 AND 20";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Conjunction().Add(Restrictions.In(\"Q\", ('Active','Closed'))).Add(Restrictions.Between(\"R\", 7, 20)))");
        }

        [TestMethod]
        public void SqlToHib_Test16()
        {
            string text =
                "GET IsTart, Quantity " +
                "FROM e2 " +
                "WHERE Max(15 + 10, 35) + ABS(-2) > 77";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET IsTart,Quantity FROM e2 .Add(Restrictions.Gt(\"(Max((15+10),35)+ABS(-2))\", 77))");
        }

        [TestMethod]
        public void SqlToHib_DataTest1()
        {
            string text =
                "GET Listing " +
                "FROM Listing l " +
                "WHERE l.EntityKey = 452 ";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Listing FROM Listing l .Add(Restrictions.Eq(\"l.EntityKey\", 452))");
        }

        [TestMethod]
        public void SqlToHib_DataTest2()
        {
            string text =
                "GET e1.Column1, e2 " +
                "FROM Entity1 e1, e1.Entity2 e2 " +
                "WHERE e1.Column2 = 'test' AND e2.Column3 = 123 " +
                "ORDER BY e2.Column4 DESC " +
                "SKIP 10 " +
                "TAKE 50 ";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET e1.Column1,e2 FROM Entity1 e1,e1.Entity2 e2 .Add(Restrictions.Conjunction().Add(Restrictions.Eq(\"e1.Column2\", 'test')).Add(Restrictions.Eq(\"e2.Column3\", 123))) .AddOrder(Order.Desc(\"e2.Column4\")) .SetFirstResult(10).SetMaxResults(50)");
        }

        [TestMethod]
        public void SqlToHib_DataTest3()
        {
            string text =
                "GET " +
                "    l.MLSListingID, l.DaysOnMarket, l.OffMarketDate, " +
                "    l.Status, l.OriginalPrice, l.SoldPrice, l.SoldPriceToOriginalPriceRatio, " +
                "    ad.Address1, ad.City, ad.Zip, s.Side, ag.EntityKey, ag.Name, o.EntityKey, o.Name " +
                "FROM Listing l, l.Address ad, l.Sides s, s.Agent ag, s.Office o " +
                "WHERE s.Side = 'SELL' and o.EntityKey = 827 ";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET l.MLSListingID,l.DaysOnMarket,l.OffMarketDate,l.Status,l.OriginalPrice,l.SoldPrice,l.SoldPriceToOriginalPriceRatio,ad.Address1,ad.City,ad.Zip,s.Side,ag.EntityKey,ag.Name,o.EntityKey,o.Name FROM Listing l,l.Address ad,l.Sides s,s.Agent ag,s.Office o .Add(Restrictions.Conjunction().Add(Restrictions.Eq(\"s.Side\", 'SELL')).Add(Restrictions.Eq(\"o.EntityKey\", 827)))");
        }

        [TestMethod]
        public void SqlToHib_DataTest4()
        {
            string text =
                "GET Listing.Sides.Agent, Firm.Office.Agent " +
                "FROM Listing l " +
                "WHERE l.EntityKey = 452 and l.EntityKey <= 24 OR l.EntityKey < 5 or " +
                "l.EntityKey != 12 * 4 or l.EntityKey != 'test' AND l.EntityKey between 4 and 7 and l.EntityKey is NULL";
            var result = new SqlParser().RunSqlToHib(text);
            Assert.IsTrue(result == "GET Listing.Sides.Agent,Firm.Office.Agent FROM Listing l .Add(Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(Restrictions.Eq(\"l.EntityKey\", 452)).Add(Restrictions.Le(\"l.EntityKey\", 24))).Add(Restrictions.Lt(\"l.EntityKey\", 5)).Add(Restrictions.Not(Restrictions.Eq(\"l.EntityKey\", (12*4)))).Add(Restrictions.Conjunction().Add(Restrictions.Not(Restrictions.Eq(\"l.EntityKey\", 'test'))).Add(Restrictions.Between(\"l.EntityKey\", 4, 7)).Add(Restrictions.IsNull(\"l.EntityKey\"))))");
        }
    }
}
