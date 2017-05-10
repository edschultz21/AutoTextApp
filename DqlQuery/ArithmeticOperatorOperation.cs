using NHibernate.Criterion;
using NHibernate.Type;
using System;

namespace DqlHelpers
{
    public class ArithmeticOperatorProjection : OperatorProjection
    {
        public ArithmeticOperatorProjection(string op, IType returnType, params IProjection[] args)
            : base(op, returnType, args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentOutOfRangeException("args", args.Length, "Requires at least 2 projections");
            }
        }

        public override string[] AllowedOperators
        {
            get { return new[] { "+", "-", "*", "/", "%" }; }
        }
    }

    //public class BitwiseOperatorProjection : OperatorProjection
    //{
    //    public BitwiseOperatorProjection(string op, IType returnType, params IProjection[] args)
    //        : base(op, returnType, args)
    //    {
    //        if (args.Length < 2)
    //            throw new ArgumentOutOfRangeException("args", args.Length, "Requires at least 2 projections");
    //    }

    //    public override string[] AllowedOperators
    //    {
    //        get { return new[] { "&", "|", "^" }; }
    //    }
    //}
}
