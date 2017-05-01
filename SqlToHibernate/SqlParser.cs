using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace SqlToHibernate
{
    public class SqlParser
    {
        public bool OutputExpressionTree { get; set; }

        private string RunVisitor(SqlHibBaseVisitor<string> visitor, string text)
        {
            var result = string.Empty;

            // This section reads the input, does its magic, and parses the input.
            AntlrInputStream input = new AntlrInputStream(text);
            SqlHibLexer lexer = new SqlHibLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            SqlHibParser parser = new SqlHibParser(tokens);
            parser.RemoveErrorListeners();
            var errorListener = new ErrorListener();
            parser.AddErrorListener(errorListener);
            IParseTree tree = parser.sql();

            // Antlr4 works off of visitors. In this visitor we walk the tree and calculate
            // the final result of the input.
            if (!OutputExpressionTree)
            {
                if (errorListener.ErrorList.Count > 0)
                {
                    result = string.Join(Environment.NewLine, errorListener.ErrorList);
                }
                else
                {
                    result = visitor.Visit(tree);
                }
            }
            else
            {
                ParseTreeWalker walker = new ParseTreeWalker();
                var listener = new SqlHibListener();
                walker.Walk(listener, tree);

                result = tree.ToStringTree(parser);
            }

            return result.Trim();
        }

        public string RunSqlToHib(string text)
        {
            var result = RunVisitor(new SqlToHibernateVisitor(), text);

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
