using System;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data;

namespace TenK.InfoSparks.Common.AnalysisServices
{
    public class SSASConnection
    {
        public const int RECONNECT_TIME_SECONDS = 20;
        private AdomdConnection _conn;
        private readonly string _connectionString;
        private DateTime _lastRetry;
        private string _currentDatabase;
        private AdomdCommand _cmd = null;
        private ulong _cmdNumber = 0;

        private readonly object _cancelationLock = new object();
        private bool _cancelationRequested = false;

        public const string ADOMD_CANCELATION_MESSAGE = "Server: The operation was cancelled by the user.";

        /// <summary>
        /// Creates SSAS Connection based on connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public SSASConnection(String connectionString)
        {
            _connectionString = connectionString;
            TryConnect();    
        }

        public string SessionID
        {
            get { return _conn != null ? _conn.SessionID : null;  }
        }

        /// <summary>
        /// Creates SSAS Connection String based on server, database and any other options
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="catalog"></param>
        /// <param name="otherOptions"></param>
        public static string BuildSSASConnectionString(string dataSource, string catalog, string otherOptions)
        {
            return String.Format("Data Source={0};Catalog={1};{2}",dataSource,catalog,otherOptions);            
        }

        /// <summary>
        /// Opens connection (used in constructor)
        /// </summary>
        private void OpenConnection()
        {
            _conn = new AdomdConnection(_connectionString);
            _conn.Open();

            if (String.IsNullOrWhiteSpace(_currentDatabase))
            {
                _currentDatabase = _conn.Database;
            }
            else
            {
                _conn.ChangeDatabase(_currentDatabase);
            }
        }

        /// <summary>
        /// Closes and cleans up the connection
        /// </summary>
        private void CloseConnection()
        {
            if (_conn == null) return;

            try
            {
                _conn.Close();
            }
            catch
            { }
            _conn = null;
        }


        /// <summary>
        /// Changes connections current database (if required)
        /// </summary>
        /// <param name="newDatabase"></param>
        public bool ChangeDatabase(string newDatabase)
        {
            bool result = false;

            RunCommand(connection =>
                {
                    if (_currentDatabase.Equals(newDatabase)) return;

                    connection.ChangeDatabase(newDatabase);
                    _currentDatabase = connection.Database;
                    result = true;
                });


            return result;            
        }

        private void RunCommand(Action<AdomdConnection> action)
        {
            int iteration = 0;
            SSASException exception = null;

            do
            {
                // Reopen connection if closed
                CheckConnection();

                try
                {
                    // Run the code
                    action(_conn);

                    // Reset the exception on success
                    exception = null;
                }
                catch (AdomdConnectionException e)
                {
                    CloseConnection();
                    exception = new SSASServerException(e.Message);
                }
                catch (AdomdException e)
                {
                    CloseConnection();
                    // Sometimes the message sent back is doubled dunno why
                    // but to ensure correctness, we'll only look at the important characters
                    if (e.Message.Substring(0, ADOMD_CANCELATION_MESSAGE.Length) == ADOMD_CANCELATION_MESSAGE)
                    {
                        exception = new SSASCancelException(e.Message);
                    }
                    else
                    {
                        exception = new SSASDatabaseException(e.Message);
                    }
                    iteration = 2;
                }
                catch (SSASCancelException e)
                {
                    CloseConnection();
                    exception = e;
                    iteration = 2;
                }
                finally
                {
                    iteration++;
                }
            } while (exception != null && iteration < 2);

            lock (_cancelationLock)
            {
                _cancelationRequested = false;
            }

            if (exception != null) throw exception;
        }

        public CellSet ExecuteQueryCS(String mdxQuery)
        {
            CellSet cs = null;
            RunCommand(connection =>
            {
                lock (_cancelationLock)
                {
                    if (_cancelationRequested)
                    {
                        throw new SSASCancelException(ADOMD_CANCELATION_MESSAGE);
                    }
                    _cmdNumber++;
                    _cmd = new AdomdCommand(mdxQuery, connection);
                }

                try
                {
                    cs = _cmd.ExecuteCellSet();
                }
                finally
                {
                    lock (_cancelationLock)
                    {
                        _cmd = null;
                    }
                }
            });

            return cs;           
        }

        public void ExecuteNonQuery(string data)
        {
            RunCommand(connection =>
            {
                (new AdomdCommand(data, connection)).ExecuteNonQuery();
            });
        }

        public DataTable ExecuteQueryDT(String query)
        {
            DataTable dt = null;
            RunCommand(connection =>
            {
                AdomdDataAdapter da = new AdomdDataAdapter(query, _conn);
                dt = new DataTable();
                da.Fill(dt);
                
            });

            return dt;
        }

        private void CheckConnection()
        {
            if (_conn == null && !TryConnect())
            {
                throw new SSASServerException("Unable to connect to " + _connectionString);
            }
        }

        private bool TryConnect()
        {            
            if (DateTime.Now.Subtract(_lastRetry).TotalSeconds <= RECONNECT_TIME_SECONDS) return false;
            try
            {
                OpenConnection();                
            }
            catch (AdomdException e)
            {
                CloseConnection();
                _lastRetry = DateTime.Now;
            }
            return _conn != null;
        }

       
        public void Close()
        {
            CloseConnection();
        }


        public void Cancel()
        {
            lock (_cancelationLock)
            {
                _cancelationRequested = true;
            }

            ulong cmdNumber = _cmdNumber;
            AdomdCommand cmd = _cmd;
            while (cmd != null && cmdNumber == _cmdNumber)
            {
                cmd.Cancel();
                System.Threading.Thread.Sleep(100);
                cmd = _cmd;
            }
        }
    } 
}
