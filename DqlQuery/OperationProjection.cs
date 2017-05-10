using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;
using System.Collections.Generic;

namespace DqlHelpers
{
    public abstract class OperatorProjection : SimpleProjection
    {
        private readonly IProjection[] _args;
        private readonly IType _returnType;

        private string _op;
        private string Op
        {
            get { return _op; }
            set
            {
                var trimmed = value.Trim();
                if (System.Array.IndexOf(AllowedOperators, trimmed) == -1)
                {
                    throw new ArgumentOutOfRangeException("value", trimmed, "Not allowed operator");
                }
                _op = " " + trimmed + " ";
            }
        }

        public abstract string[] AllowedOperators { get; }

        protected OperatorProjection(string op, IType returnType, params IProjection[] args)
        {
            Op = op;
            _returnType = returnType;
            _args = args;
        }

        public override SqlString ToSqlString(
            ICriteria criteria, 
            int position, 
            ICriteriaQuery criteriaQuery, 
            IDictionary<string, IFilter> enabledFilters
        )
        {
            SqlStringBuilder sb = new SqlStringBuilder();
            sb.Add("(");

            for (int i = 0; i < _args.Length; i++)
            {
                int loc = (position + 1) * 1000 + i;
                SqlString projectArg = GetProjectionArgument(criteriaQuery, criteria, _args[i], loc, enabledFilters);
                sb.Add(projectArg);

                if (i < _args.Length - 1)
                {
                    sb.Add(Op);
                }
            }
            sb.Add(")");
            sb.Add(" as ");
            sb.Add(GetColumnAliases(position)[0]);

            return sb.ToSqlString();
        }

        private static SqlString GetProjectionArgument(
            ICriteriaQuery criteriaQuery, 
            ICriteria criteria,
            IProjection projection, 
            int loc,
            IDictionary<string, IFilter> enabledFilters
        )
        {
            SqlString sql = projection.ToSqlString(criteria, loc, criteriaQuery, enabledFilters);
            return NHibernate.SqlCommand.SqlStringHelper.RemoveAsAliasesFromSql(sql);
        }

        public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return new IType[] { _returnType };
        }

        public override bool IsAggregate
        {
            get { return false; }
        }

        public override bool IsGrouped
        {
            get
            {
                foreach (IProjection projection in _args)
                {
                    if (projection.IsGrouped)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            SqlStringBuilder buffer = new SqlStringBuilder();
            foreach (IProjection projection in _args)
            {
                if (projection.IsGrouped)
                {
                    buffer.Add(projection.ToGroupSqlString(criteria, criteriaQuery, enabledFilters)).Add(", ");
                }
            }
            if (buffer.Count >= 2)
            {
                buffer.RemoveAt(buffer.Count - 1);
            }
            return buffer.ToSqlString();
        }
    }
}
