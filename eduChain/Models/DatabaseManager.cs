using Npgsql;
using System.Data;

namespace eduChain.Models
{
    public static class DatabaseManager
    {
        private static readonly string ConnectionString = "User Id=postgres.wcbvpqecetfhnfphtmae;Password=notthatexcellent3224;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;";
        
        private static IDbConnection _connection;

        public static IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new NpgsqlConnection(ConnectionString);
                }
                return _connection;
            }
        }

        public static void OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public static void CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }
    }
}
