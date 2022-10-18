using Microsoft.Data.SqlClient;

namespace TabloidCLI.Repositories
{
    public class DatabaseConnector
    {
        private readonly string _connectionString;
        protected SqlConnection Connection => new SqlConnection(_connectionString);

        public DatabaseConnector(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
