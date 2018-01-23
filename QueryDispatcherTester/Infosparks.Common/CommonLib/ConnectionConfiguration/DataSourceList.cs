using System.Collections.ObjectModel;

namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    class DataSourceList : KeyedCollection<string, DataSource>
    {
        protected override string GetKeyForItem(DataSource item)
        {
            return item.DataSourceKey;
        }
    }
}
