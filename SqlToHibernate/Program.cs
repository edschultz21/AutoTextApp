using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace SqlToHibernate
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                RunSqlToCriteria(session);
            }

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("DONE!");
            //Console.ReadLine();
        }

        static void RunCustomerOrderTest()
        {
            string text =
                "GET " +
                "   p.FirstName, p.LastName, c.City, s.FirstName, s.LastName, p.ProductName " +
                "FROM Person p, p.Customer c, c.SalesPerson s, c.ProductOrder p " +
                "WHERE p.Quantity > 1 ";

            //var sqlParser = new SqlParser();
            //sqlParser.OutputExpressionTree = false;
            //Run(sqlParser, text);

            new CustomerOrdering().Run();
        }

        static void RunSqlToHib()
        {
            string text =
                "GET " +
                "    l.MLSListingID, l.DaysOnMarket, l.OffMarketDate, " +
                "    l.Status, l.OriginalPrice, l.SoldPrice, l.SoldPriceToOriginalPriceRatio, " +
                "    ad.Address1, ad.City, ad.Zip, s.Side, ag.EntityKey, ag.Name, o.EntityKey, o.Name " +
                "FROM Customer c, c.Address ad, l.Sides s, s.Agent ag, s.Office o " +
                "WHERE s.Side = 'SELL' and o.EntityKey = 827 ";
            //string text =
            //    "GET e1.Column1, e2.Column2 " +
            //    "FROM Entity1 e1, e1.Entity2 e2 " +
            //    //"WHERE Q=5 ";
            //    //"WHERE Q=5 OR R!=8 ";
            //    //"WHERE NOT e1.Column1 BETWEEN 7 AND 70 AND e2.Column2 > 150 " +
            //    "WHERE Max(15 + 10, 35) + ABS(-2) > 77";
            //    //"WHERE Q NOT LIKE 'rt%' ";
            //    //"WHERE Q is not null ";
            //    //"ORDER BY e1.Column1 ASC, e2.Column2 DESC " +
            //    //"TAKE 10 SKIP 20 ";

            var sqlParser = new SqlParser();
            sqlParser.OutputExpressionTree = false;

            Console.WriteLine($"Text: {text}");
            Console.WriteLine();

            var result = sqlParser.RunSqlToHib(text);

            Console.WriteLine($"Result: {result}");
            Console.WriteLine();
        }

        static void Test(ISession session)
        {
            var criteria = session
                .CreateCriteria("Listing", "l")
                .CreateCriteria("l.Sides", "s")
                .Add(Restrictions.Eq("s.Side", "SELL"))
                .AddOrder(Order.Asc("s.Side"))
                .AddOrder(Order.Desc("l.Sides"))
                .SetMaxResults(20);

            var ezs = criteria.List();
        }

        static void RunSqlToCriteria(ISession session)
        {
            //Test(session);
            string text = 
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL'";

            Console.WriteLine($"Text: {text}");
            Console.WriteLine();

            var sqlParser = new SqlParser();
            //var expressionTree = sqlParser.GetExpressionTree(text);
            //Console.WriteLine(expressionTree);

            var criteria = (ICriteria)sqlParser.RunSqlToCriteria(text, session);
            var results = criteria.List();
        }
    }

}

