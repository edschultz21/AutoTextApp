using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Collection.Generic;
using NHibernate.Proxy;

namespace NHibernate_test
{
    class EntityRetreiver
    {
        private readonly ISession _session;

        public EntityRetreiver(ISession session)
        {
            _session = session;
        }

        public void GetMatching(IList list, IList<string> pathsList)
        {
            var cache = new List<int>();
            var path = new LinkedList<string>();
            path.AddLast("");
            CleanUp(list, pathsList, cache);
        }

        public void CleanUp(object o, IEnumerable<string> pathsList, List<int> cache)
        {
            if (cache.Contains(o.GetHashCode())) return;



            // Add current object & save the list length
            cache.Add(o.GetHashCode());
            var cacheLength = cache.Count;

            if (o is IList)
            {
                foreach (var i in (IList)o)
                {
                    cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                    // When iterating list, do not change the path
                    CleanUp(i, pathsList, cache);
                }
            }
            else if (o is IDictionary)
            {
                var d = (IDictionary)o;
                var idColumnName = "";

                // Get type of the current object
                if (d.Contains("$type$"))
                {
                    var entityType = d["$type$"].ToString();
                    idColumnName = _session.SessionFactory.GetClassMetadata(entityType).IdentifierPropertyName;
                }
                
                foreach (var p in d.Keys.Cast<string>().ToArray())
                {

                    if (p.StartsWith("_") || p == "$type$" || p == idColumnName)
                    {
                        d.Remove(p);
                    }
                    else
                    {
                        var val = d[p];

                        if (val.IsProxy() || val is ISet<object>)
                        {
                            // Need to check if we've been asked for it
                            var reducedPath = pathsList.Where(t => t.StartsWith(p + ".") || t == p).ToList();
                            if (reducedPath.Any())
                            {
                                var subPaths =
                                    reducedPath.Select(t => t.Remove(0, t.Length == p.Length ? p.Length : p.Length + 1));
                                cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                                CleanUp(d[p], subPaths, cache);
                            }
                            else
                            {
                                d.Remove(p);
                            }
                        }
                        else
                        {
                            cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                            CleanUp(d[p], pathsList, cache);
                        }
                    }
                }
            }
            else if (o is ISet<object>)
            {
                foreach (var i in ((ISet<object>)o))
                {
                    cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                    CleanUp(i, pathsList, cache);
                }
            }

        }
    }
}
