using NHibernate;
using NHibernate.Proxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
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

        public void CleanUp(object obj, IEnumerable<string> pathsList, List<int> cache)
        {
            if (cache.Contains(obj.GetHashCode())) return;

            // Add current object & save the list length
            cache.Add(obj.GetHashCode());
            var cacheLength = cache.Count;

            if (obj is IList)
            {
                foreach (var item in (IList)obj)
                {
                    cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                    // When iterating list, do not change the path
                    CleanUp(item, pathsList, cache);
                }
            }
            else if (obj is IDictionary)
            {
                var dictObj = (IDictionary)obj;
                var idColumnName = "";

                // Get type of the current object
                if (dictObj.Contains("$type$"))
                {
                    var entityType = dictObj["$type$"].ToString();
                    idColumnName = _session.SessionFactory.GetClassMetadata(entityType).IdentifierPropertyName;
                }

                foreach (var dictItem in dictObj.Keys.Cast<string>().ToArray())
                {
                    if (dictItem.StartsWith("_") || dictItem == "$type$" || dictItem == idColumnName)
                    {
                        dictObj.Remove(dictItem);
                    }
                    else
                    {
                        var val = dictObj[dictItem];

                        if (val.IsProxy() || val is ISet<object>)
                        {
                            // Need to check if we've been asked for it
                            var reducedPath = pathsList.Where(t => t.StartsWith(dictItem + ".") || t == dictItem).ToList();
                            if (reducedPath.Any())
                            {
                                var subPaths = reducedPath.Select(t => t.Remove(0, t.Length == dictItem.Length ? dictItem.Length : dictItem.Length + 1));
                                cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                                CleanUp(dictObj[dictItem], subPaths, cache);
                            }
                            else
                            {
                                dictObj.Remove(dictItem);
                            }
                        }
                        else
                        {
                            cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                            CleanUp(dictObj[dictItem], pathsList, cache);
                        }
                    }
                }
            }
            else if (obj is ISet<object>)
            {
                foreach (var item in ((ISet<object>)obj))
                {
                    cache.RemoveRange(cacheLength, cache.Count - cacheLength);
                    CleanUp(item, pathsList, cache);
                }
            }

        }
    }
}
