using System.Collections.Generic;

namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    public interface IConnectionConfiguration
    {
        /// <summary>
        /// Returns SSAS Connection String for a given server
        /// </summary>
        /// <param name="serverName">Server name</param>
        /// <returns></returns>
        string GetSSASConnectionString(string serverName);

        /// <summary>
        /// Returns SQL connection string for a given data source
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        string GetSQLConnectionString(string dataSource);

        /// <summary>
        /// Returns URI of the dispatcher for a given server
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        string GetDispatcherUri(string serverName);

        IEnumerable<Server> GetServerList();
        IEnumerable<Cube> GetCubeList();        
        string GetCubeName(string dataSource);
        IEnumerable<CubeLocation> GetCubeLocations(string dataSource);
    }
}