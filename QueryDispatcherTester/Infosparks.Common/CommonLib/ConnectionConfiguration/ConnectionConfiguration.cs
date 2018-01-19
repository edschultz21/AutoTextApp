using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace TenK.InfoSparks.Common.ConnectionConfiguration
{
    public class ConnectionConfiguration : IConnectionConfiguration
    {
        private readonly string _connDbConnectionString;

        private ServerList _serverList = new ServerList();
        private DataSourceList _dataSourceList;
        private List<Cube> _cubeList;

        public ConnectionConfiguration(string connDbConnectionString)
        {
            _connDbConnectionString = connDbConnectionString;
            ReloadSources();
        }

        public void ReloadSources()
        {
            var datatSourceList = new DataSourceList();
            var serverList = new ServerList();
            List<Cube> cubeList = null;      
            using (IDbConnection db = new SqlConnection(_connDbConnectionString))
            {
                db.Open();
                var dbserverList = db.Query<Server>(Server.SELECT_QUERY);
                foreach (var server in dbserverList)
                {
                    serverList.Add(server);
                }


                cubeList = db.Query<Cube>(Cube.SELECT_QUERY).ToList();

                var dsList = db.Query<DataSource>(DataSource.SELECT_QUERY);
                foreach (var dataSource in dsList)
                {
                    datatSourceList.Add(dataSource);
                }
            }

            _serverList = serverList;
            _dataSourceList = datatSourceList;
            _cubeList = cubeList;
        }

        public string GetSSASConnectionString(string serverName)
        {
            return _serverList[serverName].SSASConnectionString;
        }

        public string GetDispatcherUri(string serverName)
        {
            return _serverList[serverName].QueryDisparcherUri;
        }

        public IEnumerable<Server> GetServerList()
        {
            return _serverList.ToList();
        }

        public IEnumerable<Cube> GetCubeList()
        {
            return _cubeList.ToList();
        }

        public string GetSQLConnectionString(string dataSource)
        {
            return _dataSourceList[dataSource].SQLConnectionString;
        }

        public string GetCubeName(string dataSource)
        {
            return _dataSourceList[dataSource].CubeName;
        }

        public IEnumerable<CubeLocation> GetCubeLocations(string dataSource)
        {
            var result = from cube in _cubeList
                         join server in _serverList on cube.ServerKey equals server.ServerKey
                         join source in _dataSourceList on cube.DataSourceKey equals source.DataSourceKey
                         where cube.DataSourceKey == dataSource
                         select
                             new CubeLocation()
                                 {
                                     CubeName = source.CubeName,
                                     SSASConnectionString = server.SSASConnectionString,
                                     CubeDatabase = cube.CubeDatabase
                                 };
            return result;
        }
    }
}
