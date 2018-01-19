using System.Collections.ObjectModel;

namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    class ServerList : KeyedCollection<string, Server>
    {
        protected override string GetKeyForItem(Server item)
        {
            return item.ServerKey;
        }
    }
}
