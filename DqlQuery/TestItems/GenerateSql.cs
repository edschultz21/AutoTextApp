using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Loader.Criteria;

namespace DqlQuery.TestItems
{
    public class GenerateSql
    {
        public static string GetSql(ICriteria criteria)
        {
            CriteriaImpl criteriaImpl;
            if (criteria is CriteriaImpl.Subcriteria)
            {
                criteriaImpl = (CriteriaImpl)((CriteriaImpl.Subcriteria)criteria).Parent;
            }
            else
            {
                criteriaImpl = (CriteriaImpl)criteria;
            }
            var sessionImpl = (SessionImpl)criteriaImpl.Session;
            var factory = (SessionFactoryImpl)sessionImpl.SessionFactory;
            var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);
            var loader = new CriteriaLoader((IOuterJoinLoadable)factory.GetEntityPersister(implementors[0]), factory, criteriaImpl, implementors[0], sessionImpl.EnabledFilters);

            return loader.SqlString.ToString();
        }
    }
}
