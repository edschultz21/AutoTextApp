using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlToHibernate;
using NHibernate;

namespace UnitTests
{
    /// <summary>
    /// Summary description for SqlToCriteria
    /// </summary>
    [TestClass]
    public class SqlToCriteria
    {

        private string RunCriteria(string text)
        {
            var output = string.Empty;

            var sqlParser = new SqlParser();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var criteria = (ICriteria)sqlParser.RunSqlToCriteria(text, session);
                var sql = SqlToHibernate.TestItems.GenerateSql.GetSql(criteria);
                criteria.SetFetchMode("Listing", FetchMode.Eager);
                IList results = criteria.List();

                output = OutputRows.GetValues(results);
            }

            return output;
        }


        [TestMethod]
        public void SqlToCriteria_Test1()
        {
            string text =
                "GET Listing " +
                "FROM Listing l";

            var results = RunCriteria(text);
        }

        [TestMethod]
        public void SqlToCriteria_Test2()
        {
            string text =
                "GET Listing " +
                "FROM Listing l, l.Sides s " +
                "WHERE s.Side = 'SELL'";

            var results = RunCriteria(text);
        }
    }
}
