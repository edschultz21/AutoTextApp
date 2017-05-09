using System;
using System.Collections;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using NHibernate.SqlCommand;

namespace DqlQuery
{
    public class CustomerOrdering
    {
        // Return ProductOrder information based on criteria.
        //
        // SELECT ProductName FROM ProductOrder WHERE CustomerID = 5 AND (Quantity = 6 OR Quantity = 12)
        //      AND TotalCost BETWEEN 1 and 20
        public static void Test2()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var criteria = session
                    .CreateCriteria("Listing", "l")
                    .CreateCriteria("l.Sides", "s")
                    .Add(Restrictions.Eq("Side", "Sell"));

                var results = criteria.SetResultTransformer(CriteriaSpecification.DistinctRootEntity).SetFetchMode("Listing.Sides", FetchMode.Lazy).List();

                var retriever = new EntityRetreiver(session);
                retriever.GetMatching(results, new List<string>() { "" });
            }
        }

        public void Run()
        {
            Test2();

            Console.WriteLine();
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}
