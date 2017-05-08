using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Collection.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;

namespace NHibernate_test
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration();
            config.AddFile("client1.xml");

            // Getting custom metadata for entities and properties
            var x = config.GetClassMapping("Listing");            
            Console.WriteLine(x.GetMetaAttribute("test").Value);

            ISessionFactory sf = config.Configure().BuildSessionFactory();

            dynamic l = null;
            using (ISession s = sf.OpenSession().GetSession(EntityMode.Map))
            {
                

                //l = s.CreateQuery("select u from Listing as u").List().FirstOrNull();

                //var x = l["Sides"] as PersistentGenericSet<dynamic>;

                // Querying related table
                /*var listing = s.CreateCriteria("Listing").CreateCriteria("Sides")
                    .CreateCriteria("Agent").Add(Restrictions.Like("Name", "%RJ%"))
                    .SetResultTransformer(CriteriaSpecification.DistinctRootEntity)
                    .List();*/
                // Step 1. Add all entities we will be running queries againts.
                var q = s.CreateCriteria("Listing", "l");

                q = q.CreateCriteria("l.Sides","s").Add(Restrictions.Eq("Side","Sell"));
                //q = q.CreateCriteria("s.Agent").Add(Restrictions.Eq("Name","Andrei Rjeousski"));

                l = q.SetResultTransformer(CriteriaSpecification.DistinctRootEntity).SetFetchMode("Listing.Sides",FetchMode.Lazy).List();

                

                /*using (var strWriter = new StringWriter())
                {
                    using (var jsonWriter = new PathLimitingJsonTextWriter(strWriter))
                    {
                        Func<bool> include = () => {
                            return true;                            
                        };



                        var resolver = new CustomContractResolver(include);
                        var serializer = new JsonSerializer { ContractResolver = resolver, ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
                        serializer.Serialize(jsonWriter, r);
                    }
                    var str =  strWriter.ToString();
                }*/

                /*Func<bool> include = () => {
                    return true;
                };*/

                var e = new EntityRetreiver(s);
                e.GetMatching(l, new List<string>() {""});

                
                // var listings = s.CreateQuery("select  l from Listing as l join l.Sides as s join s.Agent a where s.Side='Sell' and a.Name like 'Andrei%'");


            }

            var str = JsonConvert.SerializeObject(l, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        }

       
    }
}
