using System;
using System.Collections.Generic;

namespace SqlToHibernate
{
    class Program
    {
        static void Main(string[] args)
        {
            string text =
                "GET IsTart " +
                "FROM e2 " +
                "WHERE Q IS NOT NULL " +
                "SKIP 10 TAKE 20";
            //string text =
            //    "GET e1.Column1, e2.Column2 " +
            //    "FROM Entity1 e1, e1.Entity2 e2 " +
            //    //"WHERE Q=5 ";
            //    //"WHERE Q=5 OR R!=8 ";
            //    //"WHERE NOT e1.Column1 BETWEEN 7 AND 70 AND e2.Column2 > 150 " +
            //    "WHERE Max(15 + 10, 35) + ABS(-2) > 77";
            //    //"WHERE Q NOT LIKE 'rt%' ";
            //    //"WHERE Q is not null ";
            //    //"ORDER BY e1.Column1 ASC, e2.Column2 DESC " +
            //    //"TAKE 10 SKIP 20 ";

            var sqlParser = new SqlParser();
            sqlParser.OutputExpressionTree = false;
            Run(sqlParser, text);

            Console.WriteLine();
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }

        static void Run(SqlParser sqlParser, string text)
        {
            Console.WriteLine($"Text: {text}");
            Console.WriteLine();

            var result = sqlParser.RunSqlToHib(text);

            Console.WriteLine($"Result: {result}");
            Console.WriteLine();

            Console.WriteLine("----------------------------------------------------");
        }
    }
}

