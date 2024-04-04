using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace eduChain.Models
{
    public static class DatabaseManager
    {
        private static readonly string ConnectionString = "User Id=postgres.wcbvpqecetfhnfphtmae;Password=notthatexcellent3224;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;";

        private static NpgsqlConnection _connection;

        public static NpgsqlConnection Connection
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

        public static async Task OpenConnectionAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
        }

        public static async Task CloseConnectionAsync()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                await Connection.CloseAsync();
            }
        }

        public static async Task ExecuteNonQueryAsync(string sql)
        {
            await OpenConnectionAsync();

            using (var command = new NpgsqlCommand(sql, Connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            await CloseConnectionAsync();
        }
    }
}
