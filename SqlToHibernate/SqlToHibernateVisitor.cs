using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace SqlToHibernate
{
    public class SqlToHibernateVisitor : SqlHibBaseVisitor<string>
    {
        private enum StatementType
        {
            GET,
            FROM,
            WHERE,
            ORDER
        }

        private class EntityPath
        {
            public string Path { get; set; }
            public string Alias { get; set; }

            public override string ToString()
            {
                var result = Path + (!string.IsNullOrEmpty(Alias) ? " " + Alias : "");
                return result;
            }
        }

        private readonly Dictionary<string, string> ComparisonOperatorMap = new Dictionary<string, string>
        {
            { "=", "Eq" },
            { ">", "Gt" },
            { "<", "Lt" },
            { "<=", "Le" },
            { ">=", "Ge" },
            { "!=", "NE" },
            { "<>", "NE" },
        };


        private string _whereClause = string.Empty;
        private string _orderByClause = string.Empty;
        private string _skipTakeClause = string.Empty;
        private Dictionary<StatementType, List<EntityPath>> _entityPaths;

        public SqlToHibernateVisitor()
        {
            _entityPaths = new Dictionary<StatementType, List<EntityPath>>();
            foreach (var type in Enum.GetValues(typeof(StatementType)))
            {
                _entityPaths.Add((StatementType)type, new List<EntityPath>());
            }
        }

        #region High Level SQL

        // MAIN
        public override string Visit(IParseTree tree)
        {
            // Tree will be null for optional itemes. Saves us from doing null checks
            // all over the place.
            var result = (tree != null) ? base.Visit(tree) : "";
            return result;
        }

        // SQL
        public override string VisitSql([NotNull] SqlHibParser.SqlContext context)
        {
            base.VisitSql(context);

            // Built result string (primarily used for unit testing).
            var result = "";
            result += "GET " + string.Join(",", _entityPaths[StatementType.GET]) + " ";
            result += "FROM " + string.Join(",", _entityPaths[StatementType.FROM]) + " ";
            result += _whereClause + " ";
            result += _orderByClause + " ";
            result += _skipTakeClause + " ";

            return result.TrimEnd();
        }

        // SQL
        public override string VisitSql_stmt([NotNull] SqlHibParser.Sql_stmtContext context)
        {
            base.VisitSql_stmt(context);
            return string.Empty;
        }

        // GET
        public override string VisitGet_stmt([NotNull] SqlHibParser.Get_stmtContext context)
        {
            var paths = context.entity_path();
            foreach (var path in paths)
            {
                _entityPaths[StatementType.GET].Add(new EntityPath { Path = Visit(path) });
            }

            return string.Empty;
        }

        // FROM
        public override string VisitFrom_stmt([NotNull] SqlHibParser.From_stmtContext context)
        {
            var statements = context.entity_alias_stmt();
            foreach (var statement in statements)
            {
                var entityAlias = Visit(statement).Split(' ');
                _entityPaths[StatementType.FROM].Add(new EntityPath { Path = entityAlias[0], Alias = entityAlias.Length == 2 ? entityAlias[1] : "" });
            }

            return string.Empty;
        }

        // WHERE
        public override string VisitWhere_stmt([NotNull] SqlHibParser.Where_stmtContext context)
        {
            _whereClause = base.VisitWhere_stmt(context);
    
            return string.Empty;
        }

        // ORDER BY
        public override string VisitOrder_stmt([NotNull] SqlHibParser.Order_stmtContext context)
        {
            _orderByClause = string.Empty;
            var terms = context.ordering_term();
            foreach (var term in terms)
            {
                _orderByClause += ".AddOrder" + AddParens(Visit(term));
            }

            return string.Empty;
        }

        // SKIP / TAKE
        public override string VisitSkiptake_stmt([NotNull] SqlHibParser.Skiptake_stmtContext context)
        {
            _skipTakeClause = string.Empty;

            if (context.skip_term() != null)
            {
                _skipTakeClause += ".SetFirstResult" + AddParens(Visit(context.skip_term()));
            }
            if (context.take_term() != null)
            {
                _skipTakeClause += ".SetMaxResults" + AddParens(Visit(context.take_term()));
            }

            return string.Empty;
        }
        #endregion

        #region Where Clause
        public override string VisitSearch_condition([NotNull] SqlHibParser.Search_conditionContext context)
        {
            var searchConditions = context.search_condition_and();

            var result = searchConditions.Length > 1 ? ".Add(Restrictions.Disjunction()" : "";
            for (var i = 0; i < searchConditions.Length; i++)
            {
                result += Visit(searchConditions[i]);
            }
            result += searchConditions.Length > 1 ? ")" : "";

            return result;
        }

        public override string VisitSearch_condition_and([NotNull] SqlHibParser.Search_condition_andContext context)
        {
            var searchConditions = context.search_condition_not();

            var result = searchConditions.Length > 1 ? ".Add(Restrictions.Conjunction()" : "";
            for (var i = 0; i < searchConditions.Length; i++)
            {
                result += ".Add(" + Visit(searchConditions[i]) + ")";
            }
            result += searchConditions.Length > 1 ? ")" : "";

            return result;
        }

        public override string VisitSearch_condition_not([NotNull] SqlHibParser.Search_condition_notContext context)
        {
            var result = (context.NOT() != null)
                ? "Restrictions.Not" + AddParens(Visit(context.predicate()))
                : Visit(context.predicate());

            return result;
        }

        public override string VisitComparisonPred([NotNull] SqlHibParser.ComparisonPredContext context)
        {
            var result = "Restrictions.";
            var op = context.children[1].GetText();
            var restrictionOp = ComparisonOperatorMap[op];
            result += (restrictionOp == "NE") ? "Not(Restrictions.Eq" : restrictionOp;
            result += AddParens(AddQuotes(Visit(context.expr(0))) + ", " + Visit(context.expr(1))) + (restrictionOp == "NE" ? ")" : "");

            return result;
        }

        public override string VisitBetweenPred([NotNull] SqlHibParser.BetweenPredContext context)
        {
            var result = "Restrictions.";
            result += (context.NOT() != null) ? "Not(Restrictions." : "";
            result += "Between";
            result += AddParens(AddQuotes(Visit(context.expr(0))) + ", " + Visit(context.expr(1)) + ", " + Visit(context.expr(2)));
            result += (context.NOT() != null) ? ")" : "";

            return result;
        }

        public override string VisitInPred([NotNull] SqlHibParser.InPredContext context)
        {
            var result = "Restrictions.In";
            result += AddParens(AddQuotes(Visit(context.expr())) + ", " + AddParens(Visit(context.expr_list())));

            return result;
        }

        public override string VisitLikePred([NotNull] SqlHibParser.LikePredContext context)
        {
            var result = "Restrictions.";
            result += (context.NOT() != null) ? "Not(Restrictions." : "";
            result += "Like";
            result += AddParens(AddQuotes(Visit(context.expr(0))) + ", " + AddQuotes(Visit(context.expr(1)).Trim('\''))); // EZSTODO - Need better handling
            result += (context.NOT() != null) ? ")" : "";

            return result;
        }

        public override string VisitIsPred([NotNull] SqlHibParser.IsPredContext context)
        {
            var op = Visit(context.null_notnull()).ToUpper() == "NOTNULL" ? "IsNotNull" : "IsNull";
            var result = "Restrictions." + op + AddParens(AddQuotes(Visit(context.expr())));

            return result;
        }

        public override string VisitParensPred([NotNull] SqlHibParser.ParensPredContext context)
        {
            return "(" + Visit(context.search_condition()) + ")";
        }
        #endregion

        #region Expressions
        public override string VisitExpr([NotNull] SqlHibParser.ExprContext context)
        {
            var result = base.VisitExpr(context);
            return result;
        }

        public override string VisitNullExpr([NotNull] SqlHibParser.NullExprContext context)
        {
            return context.GetText();
        }

        public override string VisitConstantExpr([NotNull] SqlHibParser.ConstantExprContext context)
        {
            return context.GetText();
        }

        public override string VisitFunctionCallExpr([NotNull] SqlHibParser.FunctionCallExprContext context)
        {
            return base.VisitFunctionCallExpr(context);
        }

        public override string VisitFunction_call([NotNull] SqlHibParser.Function_callContext context)
        {
            var result = Visit(context.func_proc_name());
            result += AddParens(Visit(context.expr_list()));
            return result;
        }

        public override string VisitEntityPathExpr([NotNull] SqlHibParser.EntityPathExprContext context)
        {
            var result = Visit(context.entity_path());
            return result;
        }

        public override string VisitParensExpr([NotNull] SqlHibParser.ParensExprContext context)
        {
            var result = "(" + Visit(context.expr()) + ")";
            return result;
        }

        public override string VisitMulDivExpr([NotNull] SqlHibParser.MulDivExprContext context)
        {
            var op = context.children[1].GetText();

            var result = Visit(context.expr(0)) + op + Visit(context.expr(1));
            return AddParens(result);
        }

        public override string VisitUnaryExpr([NotNull] SqlHibParser.UnaryExprContext context)
        {
            var result = context.children[0].GetText() + Visit(context.expr());
            return result;
        }

        public override string VisitAddSubExpr([NotNull] SqlHibParser.AddSubExprContext context)
        {
            var op = context.children[1].GetText();

            var result = Visit(context.expr(0)) + op + Visit(context.expr(1));
            return AddParens(result);
        }

        public override string VisitComparisonExpr([NotNull] SqlHibParser.ComparisonExprContext context)
        {
            var op = context.children[1].GetText();

            var result = Visit(context.expr(0)) + op + Visit(context.expr(1));
            return AddParens(result);
        }

        public override string VisitExpr_list([NotNull] SqlHibParser.Expr_listContext context)
        {
            var result = string.Empty;
            var exprList = context.expr();
            foreach (var expr in exprList)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ",";
                }
                result += Visit(expr);
            }

            return result;
        }
        #endregion

        #region Supporting Cast
        public override string VisitNull_notnull([NotNull] SqlHibParser.Null_notnullContext context)
        {
            return context.GetText();
        }

        public override string VisitEntity_path([NotNull] SqlHibParser.Entity_pathContext context)
        {
            var result = "";
            var segments = context.segment_name();
            foreach (var segment in segments)
            {
                if (result != string.Empty)
                {
                    result += ".";
                }
                result += Visit(segment);
            }

            return result;
        }

        public override string VisitEntity_alias_stmt([NotNull] SqlHibParser.Entity_alias_stmtContext context)
        {
            var result = Visit(context.entity_path());
            var aliasName = Visit(context.path_alias());

            if (!string.IsNullOrEmpty(aliasName))
            {
                result += $" {aliasName}";
            }

            return result;
        }

        public override string VisitOrdering_term([NotNull] SqlHibParser.Ordering_termContext context)
        {
            var result = (context.DESC() != null) ? "Order.Desc" : "Order.Asc";
            result += AddParens(AddQuotes(Visit(context.entity_path())));

            return result;
        }

        public override string VisitSkip_term([NotNull] SqlHibParser.Skip_termContext context)
        {
            var result = context.NUMERAL().GetText();
            return result;
        }

        public override string VisitTake_term([NotNull] SqlHibParser.Take_termContext context)
        {
            var result = context.NUMERAL().GetText();
            return result;
        }
        #endregion

        #region Naming
        public override string VisitAny_name([NotNull] SqlHibParser.Any_nameContext context)
        {
            var result = string.Empty;
            if (context.Start.Type == SqlHibParser.IDENTIFIER)
            {
                result = context.IDENTIFIER().Symbol.Text;
            }
            else if (context.Start.Type == SqlHibParser.STRING_LITERAL)
            {
                result = context.IDENTIFIER().Symbol.Text;
            }
            else
            {
                result = base.VisitAny_name(context);
            }

            return result;
        }

        public override string VisitSegment_name([NotNull] SqlHibParser.Segment_nameContext context)
        {
            return context.GetText();
        }

        public override string VisitFunc_proc_name([NotNull] SqlHibParser.Func_proc_nameContext context)
        {
            return context.GetText();
        }

        public override string VisitPath_alias([NotNull] SqlHibParser.Path_aliasContext context)
        {
            return context.GetText();
        }
        #endregion

        #region Helpers
        private string AddParens(string text)
        {
            return "(" + text + ")";
        }

        private string AddQuotes(string text)
        {
            return "\"" + text + "\"";
        }
        #endregion
    }
}
