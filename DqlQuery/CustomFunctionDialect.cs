using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace DqlQuery
{
    class CustomFunctionDialect : MsSql2012Dialect
    {
        public CustomFunctionDialect()
        {
            RegisterFunction("getdate", new NoArgSQLFunction("getdate", NHibernateUtil.DateTime, true));
            RegisterFunction("rtrim", new SQLFunctionTemplate(NHibernateUtil.String, "rtrim(?1)"));
            RegisterFunction("powertestfunc", new SQLFunctionTemplate(NHibernateUtil.Int32, "dbo.powertestfunc(?1, ?2)"));
        }
    }
}
