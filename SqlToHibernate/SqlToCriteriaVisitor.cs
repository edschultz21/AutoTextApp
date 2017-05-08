using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using NHibernate;
using NHibernate.Criterion;

// EZSTODO
// - Aliases
// - Multiple Criteria (subqueries)
// - Function calls
// - Unit tests???

namespace SqlToHibernate
{
    public class SqlToCriteriaVisitor : SqlHibBaseVisitor<object>
    {
        private class SkipTake
        {
            public int SkipAmount { get; set; }
            public int TakeAmount { get; set; }
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

        private ISession _session;

        public SqlToCriteriaVisitor(ISession session)
        {
            _session = session;
        }

        #region High Level SQL

        // MAIN - Called EVERY time we traverse any subtree (ie, all of them).
        public override dynamic Visit(IParseTree tree)
        {
            // Tree will be null for optional itemes. Saves us from doing null checks all over the place.
            return (tree != null) ? base.Visit(tree) : null;
        }

        // SQL
        // Will return an ICriteria that can be executed.
        public override dynamic VisitSql([NotNull] SqlHibParser.SqlContext context)
        {
            // Must return a criteria object that can be executed. 
            // Note that the first element in the FROM statement is used as the first criteria
            // while all the rest are subqueries/joins.
            // Also, all of the GET items are used AFTER the criteria is executed.
            return Visit(context.sql_stmt());
        }

        // SQL
        public override dynamic VisitSql_stmt([NotNull] SqlHibParser.Sql_stmtContext context)
        {
            // Traverse all of the possible SQL statements.
            // DO NOT cast here as many of these can be null.
            var getEntities = Visit(context.get_stmt());
            var fromEntities = Visit(context.from_stmt());
            var whereStatement = Visit(context.where_stmt());
            var orderByStatement = Visit(context.order_stmt());
            var skipTakeStatement = Visit(context.skiptake_stmt());

            ICriteria criteria = null;

            // Build the criteria (tables that we want data from).
            // Note that this "simplistic" way does not work as it returns all rows
            // for all subqueries. Need to figure out how to fix that (EZSTODO).
            foreach (var entity in fromEntities)
            {
                criteria = CreateCriteria(criteria, entity);
            }

            // This handles all of our predicates.
            if (whereStatement != null)
            {
                criteria.Add(whereStatement);
            }

            // Add any order by clauses.
            if (orderByStatement != null)
            {
                foreach (var order in orderByStatement)
                {
                    criteria = criteria.AddOrder(order);
                }
            }

            // Finally, add any SKIP/TAKE clauses.
            if (skipTakeStatement != null)
            {
                if (skipTakeStatement.SkipAmount != 0)
                {
                    criteria.SetFirstResult(skipTakeStatement.SkipAmount);
                }
                if (skipTakeStatement.TakeAmount != 0)
                {
                    criteria.SetMaxResults(skipTakeStatement.TakeAmount);
                }
            }

            return criteria;
        }

        // GET
        // Currently we just build a list of items that we want to retrieve. This whole list
        // will be used to determine what needs to be returned.
        public override dynamic VisitGet_stmt([NotNull] SqlHibParser.Get_stmtContext context)
        {
            var entityPaths = new List<EntityPath>();
            var paths = context.entity_path();
            foreach (var path in paths)
            {
                entityPaths.Add(new EntityPath { Path = Visit(path).ToString() });
            }

            return entityPaths;
        }

        // FROM
        // These values are simply stored and will be used later to create the appropriate
        // joins and subqueries to get the exact data we want.
        public override dynamic VisitFrom_stmt([NotNull] SqlHibParser.From_stmtContext context)
        {
            var entityPaths = new List<EntityPath>();
            var statements = context.entity_alias_stmt();
            foreach (var statement in statements)
            {
                var entityAlias = Visit(statement).ToString().Split(' ');
                entityPaths.Add(new EntityPath { Path = entityAlias[0], Alias = entityAlias.Length == 2 ? entityAlias[1] : "" });
            }

            return entityPaths;
        }

        // WHERE
        // EVERY item is created through the Restrictions class. Each method in the Restrictions
        // class returns some form of ICriterion object. The aggregation of all of those is
        // what is returned here.
        //
        // Note that this IS NOT a Criteria object. There is no way to pass that around without
        // making it global. Hence, we build and aggregate the sub pieces and add this to
        // the Criteria object later (when we build the complete SQL statement).
        //
        // Note that in many places one of the terms (left hand side) MUST be a property
        // while the other term MUST be a value. Note that this is NOT enforced by the 
        // grammar or the visitor. This will be a SQL execution error when we run the criteria.
        public override dynamic VisitWhere_stmt([NotNull] SqlHibParser.Where_stmtContext context)
        {
            var whereCriterion = (ICriterion)base.VisitWhere_stmt(context);

            return whereCriterion;
        }

        // ORDER BY
        public override dynamic VisitOrder_stmt([NotNull] SqlHibParser.Order_stmtContext context)
        {
            var orderByStatement = new List<Order>();
            var terms = context.ordering_term();
            foreach (var term in terms)
            {
                orderByStatement.Add((Order)Visit(term));
            }

            return orderByStatement;
        }

        // SKIP / TAKE
        public override dynamic VisitSkiptake_stmt([NotNull] SqlHibParser.Skiptake_stmtContext context)
        {
            var skipAmount = 0;
            var takeAmount = 0;

            if (context.skip_term() != null)
            {
                skipAmount = (int)Visit(context.skip_term());
            }
            if (context.take_term() != null)
            {
                takeAmount = (int)Visit(context.take_term());
            }

            return new SkipTake { SkipAmount = skipAmount, TakeAmount = takeAmount };
        }

        #endregion

        #region Where Statement

        // OR
        // If we have more than one then we have to join them with a "Disjunction".
        public override dynamic VisitSearch_condition([NotNull] SqlHibParser.Search_conditionContext context)
        {
            var searchConditions = context.search_condition_and();

            ICriterion restriction = null;
            Disjunction disjunction = searchConditions.Length > 1 ? Restrictions.Disjunction() : null;
            for (var i = 0; i < searchConditions.Length; i++)
            {
                restriction = (ICriterion)Visit(searchConditions[i]);
                if (disjunction != null)
                {
                    disjunction.Add(restriction);
                }
            }

            return disjunction == null ? restriction : disjunction;
        }

        // AND
        // If we have more than one then we have to join them with a "Conjunction".
        public override dynamic VisitSearch_condition_and([NotNull] SqlHibParser.Search_condition_andContext context)
        {
            var searchConditions = context.search_condition_not();

            ICriterion restriction = null;
            Conjunction conjunction = searchConditions.Length > 1 ? Restrictions.Conjunction() : null;
            for (var i = 0; i < searchConditions.Length; i++)
            {
                restriction = (ICriterion)Visit(searchConditions[i]);
                if (conjunction != null)
                {
                    conjunction.Add(restriction);
                }
            }

            return conjunction == null ? restriction : conjunction;
        }

        // NOT
        public override dynamic VisitSearch_condition_not([NotNull] SqlHibParser.Search_condition_notContext context)
        {
            var restriction = Visit(context.predicate());

            if (context.NOT() != null)
            {
                restriction = Restrictions.Not((ICriterion)restriction);
            }

            return restriction;
        }

        // +, -, *, /, %
        // Note that the left hand side MUST be a property and the right hand side MUST be a value.
        public override dynamic VisitComparisonPred([NotNull] SqlHibParser.ComparisonPredContext context)
        {
            object restriction = null;

            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            var op = context.children[1].GetText();
            switch (op)
            {
                case "=":
                    restriction = Restrictions.Eq(lhs.ToString(), rhs);
                    break;
                case ">":
                    restriction = Restrictions.Gt(lhs.ToString(), rhs);
                    break;
                case ">=":
                    restriction = Restrictions.Ge(lhs.ToString(), rhs);
                    break;
                case "<":
                    restriction = Restrictions.Lt(lhs.ToString(), rhs);
                    break;
                case "<=":
                    restriction = Restrictions.Le(lhs.ToString(), rhs);
                    break;
                case "!=":
                case "<>":
                    restriction = Restrictions.Not(Restrictions.Eq(lhs.ToString(), rhs));
                    break;
            }

            return restriction;
        }

        // BETWEEN
        // Note that first term MUST be a property while the between terms MUST be values.
        public override dynamic VisitBetweenPred([NotNull] SqlHibParser.BetweenPredContext context)
        {
            object restriction = Restrictions.Between(Visit(context.expr(0)).ToString(), Visit(context.expr(1)), Visit(context.expr(2)));
            if (context.NOT() != null)
            {
                restriction = Restrictions.Not((ICriterion)restriction);
            }

            return restriction;
        }

        // IN
        // Note that first term MUST be a property while the IN terms MUST be values.
        public override dynamic VisitInPred([NotNull] SqlHibParser.InPredContext context)
        {
            var values = (List<object>)Visit(context.expr_list());
            return Restrictions.In(Visit(context.expr()).ToString(), values);
        }

        // LIKE
        // Note that the first term MUST be a property while value MUST be a string literal.
        public override dynamic VisitLikePred([NotNull] SqlHibParser.LikePredContext context)
        {
            return Restrictions.Like(Visit(context.expr(0)).ToString(), Visit(context.expr(1)));
        }

        // IS NULL / IS NOT NULL
        public override dynamic VisitIsPred([NotNull] SqlHibParser.IsPredContext context)
        {
            object restriction = null;

            if (Visit(context.null_notnull()).ToString().ToUpper() == "NOTNULL")
            {
                restriction = Restrictions.IsNotNull(Visit(context.expr()).ToString());
            }
            else
            {
                restriction = Restrictions.IsNull(Visit(context.expr()).ToString());
            }

            return restriction;
        }

        public override dynamic VisitParensPred([NotNull] SqlHibParser.ParensPredContext context)
        {
            return Visit(context.search_condition());
        }

        #endregion

        #region Expressions

        public override dynamic VisitExpr([NotNull] SqlHibParser.ExprContext context)
        {
            return base.VisitExpr(context);
        }

        // NULL
        public override dynamic VisitNullExpr([NotNull] SqlHibParser.NullExprContext context)
        {
            // The ICriterion treats this as NULL and not "NULL".
            return context.GetText();
        }

        // Constant
        // Returns the appropriate type. That is, string, int, float, or decimal.
        public override dynamic VisitConstantExpr([NotNull] SqlHibParser.ConstantExprContext context)
        {
            return Visit(context.constant());
        }

        // Function Call (EZSTODO)
        public override dynamic VisitFunctionCallExpr([NotNull] SqlHibParser.FunctionCallExprContext context)
        {
            return base.VisitFunctionCallExpr(context);
        }

        // Function Call (EZSTODO)
        public override dynamic VisitFunction_call([NotNull] SqlHibParser.Function_callContext context)
        {
            var result = Visit(context.func_proc_name());
            //result += AddParens(Visit(context.expr_list())); // EZSTODO
            return result;
        }

        // Entity Path
        // Builds an entity path.
        public override dynamic VisitEntityPathExpr([NotNull] SqlHibParser.EntityPathExprContext context)
        {
            return Visit(context.entity_path());
        }

        // Parens
        public override dynamic VisitParensExpr([NotNull] SqlHibParser.ParensExprContext context)
        {
            return Visit(context.expr());
        }

        // *, /, %
        // Both sides MUST be a numeric value type. Note that because the return types are 
        // dynamic, we can simply calculate the result of these values and it will do the
        // correct thing returning the correct type.
        public override dynamic VisitMulDivExpr([NotNull] SqlHibParser.MulDivExprContext context)
        {
            var op = context.children[1].GetText();

            return Eval(Visit(context.expr(0)), Visit(context.expr(1)), op);
        }

        // - (Unary)
        // The term MUST be a numeric value type.
        public override dynamic VisitUnaryExpr([NotNull] SqlHibParser.UnaryExprContext context)
        {
            var result = Visit(context.expr());
            if (!IsNumeric(result))
            {
                throw new TypeMismatchException("Cannot evaluate statement. Invalid type specified.");
            }
            if (context.op.Text == "-")
            {
                result = -result;
            }

            return result;
        }

        // +, -
        // Both sides MUST be a numeric value type. Note that because the return types are 
        // dynamic, we can simply calculate the result of these values and it will do the
        // correct thing returning the correct type.
        public override dynamic VisitAddSubExpr([NotNull] SqlHibParser.AddSubExprContext context)
        {
            var op = context.children[1].GetText();

            return Eval(Visit(context.expr(0)), Visit(context.expr(1)), op);
        }

        // Comparison (=, <, <=, >, >=, !=, <>)
        public override dynamic VisitComparisonExpr([NotNull] SqlHibParser.ComparisonExprContext context)
        {
            var op = context.children[1].GetText();

            return Visit(context.expr(0)) + op + Visit(context.expr(1));
        }

        // Expression List
        // Returns a list of values that are part of this expression. We let the caller determine
        // what to do with them. 
        public override dynamic VisitExpr_list([NotNull] SqlHibParser.Expr_listContext context)
        {
            List<object> values = new List<object>();

            var exprList = context.expr();
            foreach (var expr in exprList)
            {
                values.Add(Visit(expr));
            }

            return values;
        }

        #endregion

        #region Supporting Cast

        // NULL / NOT NULL
        public override dynamic VisitNull_notnull([NotNull] SqlHibParser.Null_notnullContext context)
        {
            // The ICriterion treats this as a keyword and not a string literal.
            return context.GetText();
        }

        // Builds the entity path.
        public override dynamic VisitEntity_path([NotNull] SqlHibParser.Entity_pathContext context)
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

        // Entity/Alias pairs.
        public override dynamic VisitEntity_alias_stmt([NotNull] SqlHibParser.Entity_alias_stmtContext context)
        {
            var result = Visit(context.entity_path());
            var aliasName = Visit(context.path_alias());

            if (!string.IsNullOrEmpty(aliasName.ToString()))
            {
                result += $" {aliasName}";
            }

            return result;
        }

        // Ordering term (entity path ASC?DESC)
        public override dynamic VisitOrdering_term([NotNull] SqlHibParser.Ordering_termContext context)
        {
            var entityPath = Visit(context.entity_path()).ToString();
            var result = (context.ASC() == null) ? Order.Desc(entityPath) : Order.Asc(entityPath);

            return result;
        }

        // SKIP
        // MUST be numeric. Note that we do not need error checking here.
        public override dynamic VisitSkip_term([NotNull] SqlHibParser.Skip_termContext context)
        {
            int result;
            Int32.TryParse(context.NUMERAL().GetText(), out result);
            return result;
        }

        // TAKE
        // MUST be numeric. Note that we do not need error checking here.
        public override dynamic VisitTake_term([NotNull] SqlHibParser.Take_termContext context)
        {
            double result;
            Double.TryParse(context.NUMERAL().GetText(), out result);
            return result;
        }

        #endregion

        #region Naming

        public override dynamic VisitAny_name([NotNull] SqlHibParser.Any_nameContext context)
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
                result = base.VisitAny_name(context).ToString();
            }

            return result;
        }

        public override dynamic VisitSegment_name([NotNull] SqlHibParser.Segment_nameContext context)
        {
            return context.GetText();
        }

        public override dynamic VisitFunc_proc_name([NotNull] SqlHibParser.Func_proc_nameContext context)
        {
            return context.GetText();
        }

        // Determines the type of the constant, converts it into that type, and then returns the value.
        // Can return string, int, float, or real. Hence why this is dynamic.
        public override dynamic VisitConstant([NotNull] SqlHibParser.ConstantContext context)
        {
            object result;

            var hasSign = context.sign() != null && context.sign().GetText() == "-";
            if (context.NUMERAL() != null)
            {
                int parsedVal;
                Int32.TryParse(context.NUMERAL().GetText(), out parsedVal);
                result = hasSign ? -parsedVal : parsedVal;
            }
            else if (context.FLOAT() != null)
            {
                float parsedVal;
                Single.TryParse(context.FLOAT().GetText(), out parsedVal);
                result = hasSign ? -parsedVal : parsedVal;
            }
            else if (context.REAL() != null)
            {
                decimal parsedVal;
                Decimal.TryParse(context.REAL().GetText(), out parsedVal);
                result = hasSign ? -parsedVal : parsedVal;
            }
            else // LITERAL
            {
                result = context.GetText().Trim('\'');
            }

            return result;
        }

        public override dynamic VisitPath_alias([NotNull] SqlHibParser.Path_aliasContext context)
        {
            return context.GetText();
        }

        #endregion

        #region Helpers

        // Checks to see if a numbers is numeric. Note that we only care about a small subset
        // of all possibilities.
        private bool IsNumeric(dynamic number)
        {
            return number is int || number is float || number is double || number is decimal;
        }

        // Evaluates the result of two values and an operation.
        // Note that both values MUST be numeric. Otherwise we throw a runtime exception.
        private dynamic Eval(dynamic left, dynamic right, string op)
        {
            if (!IsNumeric(left) || !IsNumeric(right))
            {
                throw new TypeMismatchException("Cannot evaluate statement. Invalid type specified.");
            }

            dynamic result = 0;
            switch (op)
            {
                case "+":
                    result = left + right;
                    break;
                case "-":
                    result = left - right;
                    break;
                case "*":
                    result = left * right;
                    break;
                case "/":
                    result = left / right;
                    break;
                case "%":
                    result = left % right;
                    break;
                default:
                    throw new ArgumentException("Invalid operation specified.");
            }

            return result;
        }

        // Create Criteria
        // This can get called on a session, in which case it is the main criteria, or as
        // part of an existing criteria, in which case it is a sub criteria. As this is 
        // done for every element in the FROM statement, need a place to help build it.
        private ICriteria CreateCriteria(ICriteria criteria, EntityPath entityPath)
        {
            if (string.IsNullOrEmpty(entityPath.Alias))
            {
                if (criteria == null)
                {
                    criteria = _session.CreateCriteria(entityPath.Path);
                }
                else
                {
                    criteria = criteria.CreateCriteria(entityPath.Path);
                }
            }
            else
            {
                if (criteria == null)
                {
                    criteria = _session.CreateCriteria(entityPath.Path, entityPath.Alias);
                }
                else
                {
                    criteria = criteria.CreateCriteria(entityPath.Path, entityPath.Alias);
                }
            }

            return criteria;
        }

        #endregion
    }
}
