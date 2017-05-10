using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using NHibernate;
using NHibernate.Criterion;
using DqlHelpers;

// EZSTODO
// - Aliases
// - Multiple Criteria (subqueries)
// - Function calls

namespace DqlQuery
{
    // Note that we return dynamic as it makes the arithmetic in expressions simpler. That is,
    // if we try to use various operations on numeric values, we need to keep track of their 
    // type which in theory could make things very complex. Having a dynamic type alliviates
    // all that.
    public class DqlToCriteriaVisitor : DqlBaseVisitor<object>
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
                var result = string.IsNullOrEmpty(Alias) ? Path : Alias + "." + Path;
                return result;
            }
        }

        private ISession _session;

        public DqlToCriteriaVisitor(ISession session)
        {
            _session = session;
        }

        #region High Level DQL

        // MAIN - Called EVERY time we traverse any subtree (ie, all of them).
        public override dynamic Visit(IParseTree tree)
        {
            // Tree will be null for optional itemes. Saves us from doing null checks all over the place.
            return (tree != null) ? base.Visit(tree) : null;
        }

        // DQL
        // Will return an ICriteria that can be executed.
        public override dynamic VisitDql([NotNull] DqlParser.DqlContext context)
        {
            // Must return a criteria object that can be executed. 
            // Note that the first element in the FROM statement is used as the first criteria
            // while all the rest are subqueries/joins.
            // Also, all of the GET items are used AFTER the criteria is executed.
            return Visit(context.dql_stmt());
        }

        // DQL
        public override dynamic VisitDql_stmt([NotNull] DqlParser.Dql_stmtContext context)
        {
            // Traverse all of the possible DQL statements.
            // DO NOT cast here as many of these can be null.
            var getEntities = Visit(context.get_stmt()); // Technically not needed here.
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
        public override dynamic VisitGet_stmt([NotNull] DqlParser.Get_stmtContext context)
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
        public override dynamic VisitFrom_stmt([NotNull] DqlParser.From_stmtContext context)
        {
            var entityPaths = new List<EntityPath>();

            var mainTable = Visit(context.main_table_name());
            var tableAlias = (context.table_alias() != null) ? Visit(context.table_alias()) : "";
            entityPaths.Add(new EntityPath { Path = mainTable, Alias = tableAlias });

            var statements = context.table_alias_stmt();
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
        // (Note that this is less true now that we removed the comparison operator from expressions.)
        public override dynamic VisitWhere_stmt([NotNull] DqlParser.Where_stmtContext context)
        {
            var whereCriterion = (ICriterion)base.VisitWhere_stmt(context);

            return whereCriterion;
        }

        // ORDER BY
        public override dynamic VisitOrder_stmt([NotNull] DqlParser.Order_stmtContext context)
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
        public override dynamic VisitSkiptake_stmt([NotNull] DqlParser.Skiptake_stmtContext context)
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
        public override dynamic VisitSearch_condition([NotNull] DqlParser.Search_conditionContext context)
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
        public override dynamic VisitSearch_condition_and([NotNull] DqlParser.Search_condition_andContext context)
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
        public override dynamic VisitSearch_condition_not([NotNull] DqlParser.Search_condition_notContext context)
        {
            var restriction = Visit(context.predicate());

            if (context.NOT() != null)
            {
                restriction = Restrictions.Not((ICriterion)restriction);
            }

            return restriction;
        }

        // =, >, >=, <, <=, !=, <>
        //
        // This routine is complicated only because nhibernate makes it so. For any given comparison,
        // nhibernate gives us the following options.
        //
        //      Op(string propertyName, object value)
        //      Op(IProjection projection, object value)
        //      OpProperty(IProjection projection, string otherPropertyName)
        //      OpProperty(string propertyName, string otherPropertyName)
        //      OpProperty(IProjection lshProjection, IProjection rshProjection)
        //      OpProperty(string propertyName, IProjection rshProjection)
        //
        // The parameters are as follows:
        //      propertyName - EntityPath (eg, l.ListPrice)
        //      value - numeric, string
        //      projection - calculated value (eg, l.ListPrice / 10)
        //
        // For the "OpProperty" routines, all we need to know is if one or both of our parameters
        // are entity paths as we then need to convert them to a string. Since we are with dynamics,
        // everything will work correctly.
        //
        // For the "Op" property we need to know which parameter is the value. That is, we could have
        // "l.ListPrice = 5" or "5 = l.ListPrice". We also need to know if we have an entity path, in
        // which case we need to convert it to a string, or if we have a projection.
        public override dynamic VisitComparisonPred([NotNull] DqlParser.ComparisonPredContext context)
        {
            object restriction = null;

            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            var op = context.children[1].GetText();

            if ((lhs is EntityPath || lhs is IProjection) && !(rhs is EntityPath) && !(rhs is IProjection))
            {
                dynamic actual = (lhs is EntityPath) ? lhs.ToString() : lhs;
                restriction = VisitComparisonPredHelper(actual, rhs, op);
            }
            else if ((rhs is EntityPath || rhs is IProjection) && !(lhs is EntityPath) && !(lhs is IProjection))
            {
                switch (op)
                {
                    case ">":
                        op = "<";
                        break;
                    case ">=":
                        op = "<=";
                        break;
                    case "<":
                        op = ">";
                        break;
                    case "<=":
                        op = ">=";
                        break;
                    case "=":
                    case "!=":
                    case "<>":
                        break;
                }
                dynamic actual = (rhs is EntityPath) ? rhs.ToString() : rhs;
                restriction = VisitComparisonPredHelper(actual, lhs, op);
            }
            else
            {
                dynamic lhsActual = (lhs is EntityPath) ? lhs.ToString() : lhs;
                dynamic rhsActual = (rhs is EntityPath) ? rhs.ToString() : rhs;
                switch (op)
                {
                    case "=":
                        restriction = Restrictions.EqProperty(lhsActual, rhsActual);
                        break;
                    case ">":
                        restriction = Restrictions.GtProperty(lhsActual, rhsActual);
                        break;
                    case ">=":
                        restriction = Restrictions.GeProperty(lhsActual, rhsActual);
                        break;
                    case "<":
                        restriction = Restrictions.LtProperty(lhsActual, rhsActual);
                        break;
                    case "<=":
                        restriction = Restrictions.LeProperty(lhsActual, rhsActual);
                        break;
                    case "!=":
                    case "<>":
                        restriction = Restrictions.Not(Restrictions.EqProperty(lhsActual, rhsActual));
                        break;
                }
            }

            return restriction;
        }

        private dynamic VisitComparisonPredHelper(dynamic lhs, dynamic rhs, string op)
        {
            object restriction = null;

            switch (op)
            {
                case "=":
                    restriction = Restrictions.Eq(lhs, rhs);
                    break;
                case ">":
                    restriction = Restrictions.Gt(lhs, rhs);
                    break;
                case ">=":
                    restriction = Restrictions.Ge(lhs, rhs);
                    break;
                case "<":
                    restriction = Restrictions.Lt(lhs, rhs);
                    break;
                case "<=":
                    restriction = Restrictions.Le(lhs, rhs);
                    break;
                case "!=":
                case "<>":
                    restriction = Restrictions.Not(Restrictions.Eq(lhs, rhs));
                    break;
            }

            return restriction;
        }

        // BETWEEN
        // Note that first term MUST be a property while the between terms MUST be values.
        public override dynamic VisitBetweenPred([NotNull] DqlParser.BetweenPredContext context)
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
        public override dynamic VisitInPred([NotNull] DqlParser.InPredContext context)
        {
            var values = (List<object>)Visit(context.expr_list());
            return Restrictions.In(Visit(context.expr()).ToString(), values);
        }

        // LIKE
        // Note that the first term MUST be a property while value MUST be a string literal.
        public override dynamic VisitLikePred([NotNull] DqlParser.LikePredContext context)
        {
            return Restrictions.Like(Visit(context.expr(0)).ToString(), Visit(context.expr(1)));
        }

        // IS NULL / IS NOT NULL
        public override dynamic VisitIsPred([NotNull] DqlParser.IsPredContext context)
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

        public override dynamic VisitParensPred([NotNull] DqlParser.ParensPredContext context)
        {
            return Visit(context.search_condition());
        }

        #endregion

        #region Expressions

        public override dynamic VisitExpr([NotNull] DqlParser.ExprContext context)
        {
            return base.VisitExpr(context);
        }

        // NULL
        public override dynamic VisitNullExpr([NotNull] DqlParser.NullExprContext context)
        {
            // The ICriterion treats this as NULL and not "NULL".
            return context.GetText();
        }

        // Constant
        // Returns the appropriate type. That is, string, int, float, or decimal.
        public override dynamic VisitConstantExpr([NotNull] DqlParser.ConstantExprContext context)
        {
            return Visit(context.constant());
        }

        // Function Call (EZSTODO)
        public override dynamic VisitFunctionCallExpr([NotNull] DqlParser.FunctionCallExprContext context)
        {
            return base.VisitFunctionCallExpr(context);
        }

        // Function Call (EZSTODO)
        public override dynamic VisitFunction_call([NotNull] DqlParser.Function_callContext context)
        {
            var result = Visit(context.func_proc_name());
            //result += AddParens(Visit(context.expr_list())); // EZSTODO
            return result;
        }

        // Specific alias.entity terms.
        public override object VisitTableAliasTermExpr([NotNull] DqlParser.TableAliasTermExprContext context)
        {
            return Visit(context.table_alias_term());
        }

        // Parens
        public override dynamic VisitParensExpr([NotNull] DqlParser.ParensExprContext context)
        {
            return Visit(context.expr());
        }

        // *, /, %
        // Both sides MUST be a numeric value type. Note that because the return types are 
        // dynamic, we can simply calculate the result of these values and it will do the
        // correct thing returning the correct type.
        public override dynamic VisitMulDivExpr([NotNull] DqlParser.MulDivExprContext context)
        {
            var op = context.children[1].GetText();

            return Eval(Visit(context.expr(0)), Visit(context.expr(1)), op);
        }

        // +, -
        // Both sides MUST be a numeric value type. Note that because the return types are 
        // dynamic, we can simply calculate the result of these values and it will do the
        // correct thing returning the correct type.
        public override dynamic VisitAddSubExpr([NotNull] DqlParser.AddSubExprContext context)
        {
            var op = context.children[1].GetText();

            return Eval(Visit(context.expr(0)), Visit(context.expr(1)), op);
        }

        // - (Unary)
        // The term MUST be a numeric value type.
        public override dynamic VisitUnaryExpr([NotNull] DqlParser.UnaryExprContext context)
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

        // Expression List
        // Returns a list of values that are part of this expression. We let the caller determine
        // what to do with them. 
        public override dynamic VisitExpr_list([NotNull] DqlParser.Expr_listContext context)
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
        public override dynamic VisitNull_notnull([NotNull] DqlParser.Null_notnullContext context)
        {
            // The ICriterion treats this as a keyword and not a string literal.
            return context.GetText();
        }

        // Builds the entity path.
        public override dynamic VisitEntity_path([NotNull] DqlParser.Entity_pathContext context)
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

        // Specific alias.entity terms.
        public override dynamic VisitTable_alias_stmt([NotNull] DqlParser.Table_alias_stmtContext context)
        {
            var result = ((EntityPath)Visit(context.table_alias_term())).ToString();
            var tableAliasDefined = (context.table_alias_defined() != null) ? Visit(context.table_alias_defined()) : "";

            if (!string.IsNullOrEmpty(tableAliasDefined.ToString()))
            {
                result += $" {tableAliasDefined}";
            }

            return result;
        }

        public override object VisitTable_alias_term([NotNull] DqlParser.Table_alias_termContext context)
        {
            var segmentName = Visit(context.segment_name());
            var tableAlias = Visit(context.table_alias());

            return new EntityPath { Path = segmentName, Alias = tableAlias };
        }

        // Ordering term (entity path ASC?DESC)
        public override dynamic VisitOrdering_term([NotNull] DqlParser.Ordering_termContext context)
        {
            var entityPath = Visit(context.entity_path()).ToString();
            var result = (context.ASC() == null) ? Order.Desc(entityPath) : Order.Asc(entityPath);

            return result;
        }

        // SKIP
        public override dynamic VisitSkip_term([NotNull] DqlParser.Skip_termContext context)
        {
            return Int32.Parse(context.NUMERAL().GetText());
        }

        // TAKE
        public override dynamic VisitTake_term([NotNull] DqlParser.Take_termContext context)
        {
            return Double.Parse(context.NUMERAL().GetText());
        }

        #endregion

        #region Naming

        public override dynamic VisitAny_name([NotNull] DqlParser.Any_nameContext context)
        {
            var result = string.Empty;
            if (context.Start.Type == DqlParser.IDENTIFIER)
            {
                result = context.IDENTIFIER().Symbol.Text;
            }
            else
            {
                result = base.VisitAny_name(context).ToString();
            }

            return result;
        }

        // Determines the type of the constant, converts it into that type, and then returns the value.
        // Can return string, int, float, or real. Hence why this is dynamic.
        public override dynamic VisitConstant([NotNull] DqlParser.ConstantContext context)
        {
            object result;

            var hasSign = context.sign() != null && context.sign().GetText() == "-";

            if (context.NUMERAL() != null)
            {
                var parsedVal = Int32.Parse(context.NUMERAL().GetText());
                result = hasSign ? -parsedVal : parsedVal;
            }
            else if (context.FLOAT() != null)
            {
                var parsedVal = Single.Parse(context.FLOAT().GetText());
                result = hasSign ? -parsedVal : parsedVal;
            }
            else if (context.REAL() != null)
            {
                var parsedVal = Decimal.Parse(context.REAL().GetText());
                result = hasSign ? -parsedVal : parsedVal;
            }
            else // LITERAL
            {
                result = context.GetText().Trim('\'');
            }

            return result;
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
        //
        // Note that since we work with dynamic values, this will automatically convert
        // the result to the correct type.
        private dynamic Eval(dynamic left, dynamic right, string op)
        {
            dynamic result = 0;
            if (IsNumeric(left) && IsNumeric(right))
            {
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
            }
            else
            {
                var leftProjection = GetProjection(left);
                var rightProjection = GetProjection(right);

                // Although we know the type of the numeric (if we have one), we cannot know the type of the
                // sql columns. Let sql deal with it.
                result = new ArithmeticOperatorProjection(op, NHibernateUtil.Double, leftProjection, rightProjection);
            }

            return result;
        }

        private IProjection GetProjection(dynamic value)
        {
            IProjection projection = null;
            if (IsNumeric(value))
            {
                projection = Projections.Constant(value);
            }
            else if (value is EntityPath)
            {
                projection = Projections.Property(value.ToString());
            }
            else
            {
                projection = Projections.Property(value);
            }

            return projection;
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
