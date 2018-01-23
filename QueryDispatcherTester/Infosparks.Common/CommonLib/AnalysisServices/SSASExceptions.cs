using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenK.InfoSparks.Common.AnalysisServices
{
    public class SSASException : Exception
    {
        public SSASException(string message)
            : base(message)
        {

        }
    }

    public class SSASQueryException : SSASException
    {
        public SSASQueryException(string message)
            : base(message)
        {

        }
    }

    public class SSASDatabaseException : SSASException
    {
        public SSASDatabaseException(string message)
            : base(message)
        {

        }
    }

    public class SSASCancelException : SSASException
    {
        public SSASCancelException(string message)
            : base(message)
        {

        }
    }

    public class SSASServerException : SSASException
    {
        public SSASServerException(string message)
            : base(message)
        {

        }
    }
}
