using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace DqlQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                RunDqlToCriteria(session);
            }

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("DONE!");
            //Console.ReadLine();
        }

        static void Test(ISession session)
        {
            // Return all "Listing" elements (and any associated data).
            //var criteria1 = session
            //    .CreateCriteria("Listing", "l").Add(
            //    Subqueries.PropertyIn("Id",
            //        DetachedCriteria.ForEntityName("ListingSide", "s")
            //            .CreateCriteria("Listing")
            //            .SetProjection(Projections.Property("Listing"))
            //            .Add(Restrictions.Eq("s.Side", "SELL"))));

            //// Return all "ListingSide" (Listing Agent).
            //var criteria2 = session
            //    .CreateCriteria("ListingSide", "s").Add(
            //    Subqueries.PropertyIn("Listing",
            //        DetachedCriteria.ForEntityName("Listing", "l")
            //            .CreateCriteria("Sides")
            //            .SetProjection(Projections.Property("Id"))
            //            .Add(Restrictions.Eq("s.Side", "SELL"))));

            var criteria = session
                .CreateCriteria("Listing", "l")
                .CreateAlias("l.Sides", "s", NHibernate.SqlCommand.JoinType.InnerJoin)
                .SetResultTransformer(CriteriaSpecification.DistinctRootEntity)
                .Add(Restrictions.Eq("s.Side", "SELL"));

            var results = criteria.List();
            var output = new OutputRows(true).GetValues(results);
        }

        static void RunDqlToCriteria(ISession session)
        {
            //Test(session);
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL'";

            Console.WriteLine($"Text: {text}");
            Console.WriteLine();

            var dqlMain = new DqlMain();
            //var expressionTree = sqlParser.GetExpressionTree(text);
            //Console.WriteLine(expressionTree);

            var criteria = (ICriteria)dqlMain.GetCriteria(text, session);
            var results = criteria.List();
            var output = new OutputRows(true).GetValues(results);
        }
    }

}

