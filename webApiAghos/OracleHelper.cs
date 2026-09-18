

// OracleHelper.cs
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace webApiAghos
{
    public class OracleHelper : IOracleHelper
    {
        IConfiguration configuration;

        public OracleHelper(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = configuration.GetSection("ConnectionStrings").GetSection("ADBConnection").Value;
            var conn = new OracleConnection(connectionString);

            return conn;
        }
    }
}


