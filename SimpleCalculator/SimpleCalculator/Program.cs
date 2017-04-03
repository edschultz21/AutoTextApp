using System;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace SimpleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "(9 * (10 - 4)) + (7 * 4 - (3 * (4 + 1)))";

            // This section reads the input, does its magic, and parses the input.
            AntlrInputStream input = new AntlrInputStream(text);
            CalculatorLexer lexer = new CalculatorLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            CalculatorParser parser = new CalculatorParser(tokens);
            IParseTree tree = parser.prog();

            Console.WriteLine($"Text: {text}");
            Console.WriteLine();

            // Antlr4 works off of visitors. In this visitor we walk the tree and calculate
            // the final result of the input.
            CalculatorVisitor visitor = new CalculatorVisitor();
            var result = visitor.Visit(tree);
            Console.WriteLine("First way to process parsed tree:");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine();

            // This visitor works off of the same results but instead we output the tokenized
            // version of the input.
            Console.WriteLine("Alternate way to process parsed tree:");
            Calculator2Visitor visitor2 = new Calculator2Visitor();
            Console.WriteLine(visitor2.Visit(tree));
            Console.WriteLine();

            Console.WriteLine("Parsed expression:");
            Console.WriteLine(tree.ToStringTree(parser));
            Console.WriteLine();

            Console.ReadLine();
        }
    }

    class Calculator2Visitor : CalculatorBaseVisitor<string>
    {
        public override string VisitInt(CalculatorParser.IntContext context)
        {
            return context.INT().GetText();
        }

        public override string VisitExpr([NotNull] CalculatorParser.ExprContext context)
        {
            return base.VisitExpr(context);
        }

        public override string VisitChildren(IRuleNode node)
        {
            return base.VisitChildren(node);
        }

        public override string VisitProg([NotNull] CalculatorParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override string VisitAddSub(CalculatorParser.AddSubContext context)
        {
            string left = Visit(context.expr(0));
            string right = Visit(context.expr(1));
            if (context.op.Type == CalculatorParser.ADD)
            {
                return $"{left} ADD {right}";
            }
            else
            {
                return $"{left} SUB {right}";
            }
        }

        public override string VisitMulDiv(CalculatorParser.MulDivContext context)
        {
            string left = Visit(context.expr(0));
            string right = Visit(context.expr(1));
            if (context.op.Type == CalculatorParser.MUL)
            {
                return $"{left} MUL {right}";
            }
            else
            {
                return $"{left} DIV {right}";
            }
        }

        public override string VisitParens(CalculatorParser.ParensContext context)
        {

            return "(" + Visit(context.expr()) + ")";
        }
    }

    class CalculatorVisitor : CalculatorBaseVisitor<int>
    {
        public override int VisitInt(CalculatorParser.IntContext context)
        {
            return int.Parse(context.INT().GetText());
        }

        public override int VisitExpr([NotNull] CalculatorParser.ExprContext context)
        {
            return base.VisitExpr(context);
        }

        public override int VisitChildren(IRuleNode node)
        {
            return base.VisitChildren(node);
        }

        public override int VisitProg([NotNull] CalculatorParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override int VisitAddSub(CalculatorParser.AddSubContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == CalculatorParser.ADD)
            {
                return left + right;
            }
            else
            {
                return left - right;
            }
        }

        public override int VisitMulDiv(CalculatorParser.MulDivContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == CalculatorParser.MUL)
            {
                return left * right;
            }
            else
            {
                return left / right;
            }
        }

        public override int VisitParens(CalculatorParser.ParensContext context)
        {
            return Visit(context.expr());
        }
    }
}
