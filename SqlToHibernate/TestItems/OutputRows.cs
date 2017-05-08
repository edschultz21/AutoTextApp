using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Proxy;

namespace SqlToHibernate
{
    public class OutputRows
    {
        public static string GetValues(object obj)
        {
            var cache = new List<int>();
            StringBuilder sb = new StringBuilder();
            var level = 0;
            GetValues(sb, obj, cache, "", "", ref level);
            return sb.ToString();
        }

        private static void GetValues(StringBuilder sb, object obj, List<int> cache, string prefix, string key, ref int level)
        {
            if (obj == null || cache.Contains(obj.GetHashCode())) return;

            cache.Add(obj.GetHashCode());

            if (obj is IList)
            {
                var listObj = (IList)obj;
                prefix += key;
                foreach (var item in listObj)
                {
                    level++;
                    GetValues(sb, item, cache, prefix, "", ref level);
                    level--;

                    if (level == 0)
                    {
                        sb.AppendLine();
                        cache.Clear();
                        cache.Add(obj.GetHashCode());
                    }
                }
            }
            else if (obj is IDictionary)
            {
                var dictObj = (IDictionary)obj;
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefix += " ";
                }
                prefix += key;
                foreach (var item in dictObj.Keys)
                {
                    var sitem = item.ToString();
                    if (!sitem.StartsWith("_") && sitem != "$type$" && dictObj[item] != null)
                    {
                        if (!dictObj[item].IsProxy())
                        {
                            GetValues(sb, dictObj[item], cache, prefix, sitem, ref level);
                        }
                    }
                }
            }
            else if (obj is ISet<object>)
            {
                var setObj = (ISet<object>)obj;
                foreach (var item in setObj)
                {
                    sb.Append("(");
                    GetValues(sb, item, cache, prefix, "", ref level);
                    sb.Append(")");
                }
            }
            else
            {
                cache.Remove(obj.GetHashCode());
                sb.Append(prefix + " " + obj.ToString());
            }
        }

        private static IDictionary<string, object> CreateDictionary(string prefix, int start, int count)
        {
            IDictionary<string, object> dict = new SortedDictionary<string, object>();
            for (var i = 0; i < count; i++)
            {
                var index = i + start;
                dict.Add(prefix + index.ToString(), prefix + index.ToString() + "V");
            }
            return dict;
        }

        private static IList<object> CreateList(string prefix, int count, int dictStart, int dictCount)
        {
            IList<object> list = new List<object>();
            var index = dictStart;
            for (var i = 0; i < count; i++)
            {
                list.Add(CreateDictionary("L" + prefix, index, dictCount));
                index += dictCount;
            }
            return list;
        }
    }
}

