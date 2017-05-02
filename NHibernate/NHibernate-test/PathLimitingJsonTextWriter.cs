using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NHibernate_test
{
    class PathLimitingJsonTextWriter : JsonTextWriter
    {
        public PathLimitingJsonTextWriter(TextWriter textWriter) : base(textWriter)
        {
        }

        public override void WriteStartObject()
        {
            //Console.WriteLine(base.Path);
            base.WriteStartObject();
        }

        public override void WriteStartArray()
        {
            base.WriteStartArray();
        }

        public override void WriteEndObject()
        {
            base.WriteEndObject();
        }

        public override void WriteEndArray()
        {
            base.WriteEndArray();
        }
    }
}
