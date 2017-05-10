using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using NHibernate;

namespace DqlQuery
{
    public class DqlMain
    {
        private DqlParser GetParser(string text)
        {
            // This section reads the input, does its magic, and parses the input.
            AntlrInputStream input = new AntlrInputStream(text);
            DqlLexer lexer = new DqlLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            DqlParser parser = new DqlParser(tokens);

            return parser;
        }

        private ErrorListener AddErrorListener(DqlParser parser)
        {
            var errorListener = new ErrorListener();

            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            return errorListener;
        }

        private T RunVisitor<T>(DqlBaseVisitor<T> visitor, string text, ref string errors)
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
                result = visitor.Visit(parser.dql());
            }

            return result;
        }

        public object GetCriteria(string text, ISession session)
        {
            var errors = string.Empty;
            var result = RunVisitor<dynamic>(new DqlToCriteriaVisitor(session), text, ref errors);

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
}
