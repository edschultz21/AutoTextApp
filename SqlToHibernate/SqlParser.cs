using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using NHibernate;

namespace SqlToHibernate
{
    public class SqlParser
    {
        public bool OutputExpressionTree { get; set; }

        private SqlHibParser GetParser(string text)
        {
            // This section reads the input, does its magic, and parses the input.
            AntlrInputStream input = new AntlrInputStream(text);
            SqlHibLexer lexer = new SqlHibLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            SqlHibParser parser = new SqlHibParser(tokens);

            return parser;
        }

        private ErrorListener AddErrorListener(SqlHibParser parser)
        {
            var errorListener = new ErrorListener();

            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            return errorListener;
        }

        private T RunVisitor<T>(SqlHibBaseVisitor<T> visitor, string text, ref string errors)
        {
            T result = default(T);

            var parser = GetParser(text);
            var errorListener = AddErrorListener(parser);

            // Antlr4 works off of visitors. In this visitor we walk the tree and calculate
            // the final result of the input.
            if (errorListener.ErrorList.Count > 0)
            {
                errors = string.Join(Environment.NewLine, errorListener.ErrorList);
            }
            else
            {
                result = visitor.Visit(parser.sql());
            }

            return result;
        }

        public string RunSqlToHib(string text)
        {
            var errors = string.Empty;
            var result = RunVisitor<string>(new SqlToHibernateVisitor(), text, ref errors);
            if (errors != string.Empty)
            {
                return errors;
            }

            return result.Trim();
        }

        public object RunSqlToCriteria(string text, ISession session)
        {
            var errors = string.Empty;
            var result = RunVisitor<dynamic>(new SqlToCriteriaVisitor(session), text, ref errors);

            return result;
        }

        public string GetExpressionTree(string text)
        {
            var result = string.Empty;

            var parser = GetParser(text);
            var errorListener = AddErrorListener(parser);
            IParseTree tree = parser.sql();

            ParseTreeWalker walker = new ParseTreeWalker();
            var listener = new SqlHibListener();
            walker.Walk(listener, tree);

            result = tree.ToStringTree(parser);

            return result;
        }
    }

    class ErrorListener : IAntlrErrorListener<IToken>
    {
        public List<string> ErrorList { get; set; } = new List<string>();

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException exc)
        {
            var message = exc == null ? msg : exc.Message;
            var error = $"Error in parser at line: {line}, char: {charPositionInLine}, text: \"{offendingSymbol.Text}\", msg: {message}";
            ErrorList.Add(error);
        }
    }

    class SqlHibListener : SqlHibBaseListener
    {
    }

}
